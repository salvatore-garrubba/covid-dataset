using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Covid.Importer.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyCsvParser;

namespace Covid.Importer.Services
{
    public static class ForEachAsyncExtension
    {
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(dop)
                select Task.Run(async delegate
                {
                    using (partition)
                        while (partition.MoveNext())
                            await body(partition.Current);
                }));
        }
    }
    public class InputReaderService : IInputReaderService
    {
        private static int _produced = 0;
        private readonly ILogger<InputReaderService> _logger;
        private readonly IDecoderService _decoder;

        private readonly ImportSettings _importSettings;

        public InputReaderService(IOptions<ImportSettings> importSettings, IDecoderService decoder, ILogger<InputReaderService> logger)
        {
            _logger = logger;
            _decoder = decoder;
            _importSettings = importSettings.Value;
        }

        public static int _concurrentReadingThreads = 0;


        public async Task StartConsumingAsync(ITargetBlock<InputData> target, CancellationToken token)
        {
            string[] filePaths = Directory.GetFiles(_importSettings.SourceFolder, _importSettings.FileSearchPattern);

            if (filePaths == null || filePaths.Length == 0)
            {
                _logger.LogWarning(
                    "No files found in directory {_importSettings.SourceFolder} with search pattern {_importSettings.FileSearchPattern}"
                    ,_importSettings.SourceFolder
                    ,_importSettings.FileSearchPattern
                );
            }
            else
            {
                _logger.LogInformation("Files found to import: {filePaths.Length}", filePaths.Length); 
            }
            // foreach (string fileFullName in filePaths)
            // {
            //     await ImportFile(fileFullName, target, token);                
            // }
            int parallelTask = 1;
            if (_importSettings.ParallelCSVReaders > 1 && _importSettings.ParallelCSVReaders <= 10)
            {
                parallelTask = _importSettings.ParallelCSVReaders;
            }
            else
            {
                _logger.LogWarning("Requested number of parallel reading thread {_importSettings.ParallelCSVReaders} was out of range. Default value {parallelTask} is used", _importSettings.ParallelCSVReaders, parallelTask);
            }

            await filePaths.ForEachAsync(parallelTask, async fileFullName => await ImportFile(fileFullName, target, token));

            _logger.LogInformation("Total read items {_produced}, cancelled by user: {token.IsCancellationRequested}", _produced, token.IsCancellationRequested);

            target.Complete();
        }

        #region private helpers
        private async Task ImportFile(string fileFullName, ITargetBlock<InputData> target, CancellationToken token)
        {
            
            FileInfo fi = new FileInfo(fileFullName);
            bool isBigFile = fi.Length / 1048576 > _importSettings.CSVFileSizeInMBLimit;
            Interlocked.Increment(ref _concurrentReadingThreads);
            _logger.LogInformation("Start Importing file {fileName}. Concurrent reading tasks {_concurrentReadingThreads}. Is bigfile {isBigFile}", fi.Name, _concurrentReadingThreads, isBigFile);            
            if (isBigFile)
            {
                await ImportBigFile(fi, target, token);
            }
            else
            {
                await ImportSmallFile(fi, target, token);                
            }
            Interlocked.Decrement(ref _concurrentReadingThreads);            

            

            _logger.LogInformation("Finished importing file {fileName}. Total Items produced till now {_produced}.", fi.Name, _produced);
        }
        private async Task ImportBigFile(FileInfo fi, ITargetBlock<InputData> target, CancellationToken token)
        {
            using (StreamReader csvReader = new StreamReader(fi.FullName))
            {
                //string inputLine = await csvReader.ReadLineAsync();
                string inputLine = csvReader.ReadLine();
                while (!token.IsCancellationRequested && inputLine != null)
                {
                    var parsed = _decoder.Parse(inputLine);
                    parsed.FileName = fi.Name;

                    await PostData(target, parsed);
                    inputLine = csvReader.ReadLine();
                    //inputLine = await csvReader.ReadLineAsync();
                }               
            }
        }
        private async Task ImportSmallFile(FileInfo fi, ITargetBlock<InputData> target, CancellationToken token)
        {
            using (StreamReader csvReader = new StreamReader(fi.FullName))
            {
                string cm = await csvReader.ReadToEndAsync();

                foreach (InputData parsed in _decoder.ParseAll(cm))
                {
                    parsed.FileName = fi.Name;
                    if (token.IsCancellationRequested)
                    {                        
                        break;
                    }
                    await PostData(target, parsed);
                }

            }
        }
        private async Task PostData(ITargetBlock<InputData> target, InputData input)
        {
            while (!target.Post(input))
            {
                 _logger.LogInformation("Post data returned false. Wait and retry in a while...");
                await Task.Delay(2000);               
            };
            Interlocked.Increment(ref _produced);
            if (_produced % 100000 == 0)
                _logger.LogDebug("Items actually produced {_produced}", _produced);
        }
        #endregion
    }
}