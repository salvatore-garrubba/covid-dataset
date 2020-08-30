using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using Covid.Importer.Domain.Models;
using Covid.Importer.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Covid.Importer.Services
{
    public class ImportService : IImportService
    {
        private IInputReaderService _inputReader;
        private IDecoderService _decoder;
        private IDBPersister _dbPersister;
        private IErrorPersister _errorPersister;
        private readonly ILogger<ImportService> _logger;

        private readonly ImportSettings _importSettings;


        public ImportService(IOptions<ImportSettings> importSettings, IInputReaderService inputReader, IDecoderService decoder, IDBPersister dbPersister, IErrorPersister errorPersister, ILogger<ImportService> logger)
        {
            _inputReader = inputReader;
            _decoder = decoder;
            _dbPersister = dbPersister;
            _errorPersister = errorPersister;
            _logger = logger;
            _importSettings = importSettings.Value;
        }

        public async Task StartImportAsync(CancellationToken token)
        {
            // Step 1 - Configure the pipeline

            // make sure our complete call gets propagated throughout the whole pipeline
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            int parallelPersisters = 1;
            if (_importSettings.ParallelPersisters > 1 && _importSettings.ParallelPersisters <= 10)
            {
                parallelPersisters = _importSettings.ParallelPersisters;
            }
            else
            {
                _logger.LogWarning("Requested number of parallel reading thread {_importSettings.ParallelPersisters} was out of range. Default value {parallelTask} is used", _importSettings.ParallelPersisters, parallelPersisters);
            }
           
            // create our block configurations
            var bufferOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = _importSettings.BatchSize * 5, MaxDegreeOfParallelism = parallelPersisters };
            var batchOptions = new GroupingDataflowBlockOptions() { BoundedCapacity = _importSettings.BatchSize * 5, EnsureOrdered = false };
            var parallelizedOptions = new ExecutionDataflowBlockOptions()
            {
                BoundedCapacity = _importSettings.BatchSize * 10
                ,
                MaxDegreeOfParallelism = parallelPersisters
            };

            // define each block
            var decoderBlock = new TransformBlock<InputData, ConvertedData>(
                (InputData msg) => _decoder.Decode(msg), bufferOptions
            );


            var batchBlock = new BatchBlock<ConvertedData>(_importSettings.BatchSize, batchOptions);

            var dbPersistenceBlock = new TransformBlock<ConvertedData[], ImportResult>(async
                (ConvertedData[] messages) => await _dbPersister.PersistAsync(messages.ToList()), parallelizedOptions);

            ActionBlock<ImportResult> errorPersistenceBlock = new ActionBlock<ImportResult>(
                async (ImportResult errors) => await _errorPersister.PersistErrorsAsync(errors), parallelizedOptions);
            // link the blocks to together            
            decoderBlock.LinkTo(batchBlock, linkOptions);
            batchBlock.LinkTo(dbPersistenceBlock, linkOptions);

            dbPersistenceBlock.LinkTo(errorPersistenceBlock, linkOptions, importResult => importResult.HasError);
            dbPersistenceBlock.LinkTo(DataflowBlock.NullTarget<ImportResult>(), linkOptions);
            // Step 2 - Start consuming the csv files (the producer)
            _logger.LogInformation("Pipeline configured. Starting Import");
            var consumerTask = _inputReader.StartConsumingAsync(decoderBlock, token);//, TimeSpan.FromMilliseconds(1000), FlowControlMode.LoadShed);

            while (!consumerTask.IsCompleted)
            {
                await Task.Delay(1000);
            }

            _logger.LogInformation("Data reading is complete. Waiting for running blocks");

            // wait for all leaf blocks to finish processing their data
            await Task.WhenAll(
                dbPersistenceBlock.Completion,
                errorPersistenceBlock.Completion,
                consumerTask);

            _logger.LogInformation("All blocks are concluded");
        }
    }
}