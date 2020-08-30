using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Covid.Importer.Domain.Models;
using Covid.Importer.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System;

namespace Covid.Importer.Persistence
{
    public class FileErrorPersister : IErrorPersister
    {
        private static int _concurrentThreads = 0;

        private readonly ImportSettings _importSettings;
        private readonly ILogger<FileErrorPersister> _logger;
        private readonly IDecoderService _decoder;

        private readonly string _logFolder;
        public FileErrorPersister(IOptions<ImportSettings> importSettings, IDecoderService decoder, ILogger<FileErrorPersister> logger)
        {
            _logger = logger;
            _importSettings = importSettings.Value;
            _decoder = decoder;

            _logFolder = _importSettings.ErrorsFolder ?? "LOGS";
            if (!Path.IsPathFullyQualified(_logFolder))
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                _logFolder = Path.Combine(baseDir, _logFolder);
            }
            DirectoryInfo dInfo = new DirectoryInfo(_logFolder);
            if (_importSettings.RecreateErrorsFolder && dInfo.Exists)
            {
                dInfo.EnumerateDirectories("ExecutionOf_*").ToList().ForEach(d => d.Delete(true));                
            }
            if (!Directory.Exists(_logFolder))
            {
                Directory.CreateDirectory(_logFolder);
            };

            _logFolder = Path.Combine(_logFolder, $"ExecutionOf_{DateTime.Now.ToString("yyyyMMdd_HHmmss_fff")}");           
        }
        public async Task PersistErrorsAsync(ImportResult importResult)
        {
            int concurrentThreads = Interlocked.Increment(ref _concurrentThreads);
            if (!Directory.Exists(_logFolder))
            {
                Directory.CreateDirectory(_logFolder);
            };
            try
            {
                string logFileForCurrentBatch = Path.Combine(_logFolder, $"{importResult.Batch}.log");
                _logger.LogWarning("An error occured in a batch execution. Please see logfile {logFileForCurrentBatch} for errors.", logFileForCurrentBatch);
                IEnumerable<InputData> allInputData = importResult.Data.Select(item => _decoder.Encode(item));
                string errorMessage = "";
                if (importResult != null)
                {
                    if (importResult.OccuredException == null)
                        errorMessage = "No error message available";
                    else
                        errorMessage = importResult.OccuredException.Message;
                }

                using (StreamWriter sw = new StreamWriter(logFileForCurrentBatch, true))
                {
                    foreach (InputData inputData in allInputData)
                    {
                        await sw.WriteAsync(_decoder.CSVLine(inputData, errorMessage));
                    }
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some items could be lost and not logged: . Error occured:{ex.Message}", ex.Message);
            }
            finally
            {
                Interlocked.Decrement(ref _concurrentThreads);
            }
        }
    }
}