using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Covid.Data.Domain.Models;
using Covid.Importer.Domain.Models;
using Covid.Importer.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using BenGribaudoLLC;
using BenGribaudoLLC.IEnumerableHelpers.DataReaderAdapter;
using System.Data;
using Microsoft.Extensions.Options;

namespace Covid.Importer.Persistence
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> DistinctBy<T>(this IEnumerable<T> list, Func<T, object> propertySelector)
        {
            return list.GroupBy(propertySelector).Select(x => x.First());
        }
    }
    public class DBPersister : IDBPersister
    {
        #region sql statements
        string sqlDropAndRecreateTempTables = @"
        IF OBJECT_ID(N'tempdb..#Categories') IS NOT NULL
        BEGIN
            DROP TABLE #Categories
        END
                 
        SELECT  TOP 0 * 
        INTO #Categories
        FROM dbo.categories
        
        
        IF OBJECT_ID(N'tempdb..#Questions') IS NOT NULL
        BEGIN
            DROP TABLE #Questions
        END
                 
        SELECT  TOP 0 * 
        INTO #Questions
        FROM dbo.Questions
        
        
        IF OBJECT_ID(N'tempdb..#Answers') IS NOT NULL
        BEGIN
            DROP TABLE #Answers
        END
                 
        SELECT  TOP 0 * 
        INTO #Answers
        FROM dbo.Answers
        
        ";

        string sqlCreateIndexes = @"
        CREATE CLUSTERED INDEX ix_Id ON #Categories ([id]);
        
        CREATE CLUSTERED INDEX ix_Id ON #Questions ([id]);
        
        CREATE CLUSTERED INDEX ix_Id ON #Answers ([id]);
        ";

        string sqlInsertNewRecords = @"
        ;MERGE categories WITH (HOLDLOCK) as TARGET
        USING (SELECT * FROM #categories) as SOURCE
        ON TARGET.ID = SOURCE.ID
        WHEN NOT MATCHED THEN
            INSERT (ID)
            VALUES(SOURCE.ID);

        ;MERGE questions WITH (HOLDLOCK) as TARGET
        USING (SELECT ID, CategoryId, [Text] FROM #questions) as SOURCE
        ON TARGET.ID = SOURCE.ID
        WHEN NOT MATCHED THEN
            INSERT (ID, CategoryId, [Text])
            VALUES(SOURCE.ID, SOURCE.CategoryId, SOURCE.[Text]);


        ;MERGE answers WITH (HOLDLOCK) as TARGET
        USING (SELECT ID, QuestionID, Timestamp, [Text] FROM #answers) as SOURCE
        ON TARGET.ID = SOURCE.ID
        WHEN NOT MATCHED THEN
            INSERT (ID, QuestionID, Timestamp, [Text])
            VALUES(SOURCE.ID, SOURCE.QuestionID, SOURCE.Timestamp, SOURCE.[Text]);
        
        DROP TABLE #Categories
        DROP TABLE #Questions
        DROP TABLE #Answers" 
        ;
       
        #endregion
        private readonly ILogger<DBPersister> _logger;
        private readonly ImportSettings _importSettings;
        
        private static int _concurrentThreads = 0;

        private static int _batchOrder = 0;

        private static int _persisted = 0;

        private static int _total = 0;       

        public DBPersister(IOptions<ImportSettings> importSettings, ILogger<DBPersister> logger)
        {
            _logger = logger;
            _importSettings = importSettings.Value;                        
        }
        public async Task<ImportResult> PersistAsync(IList<ConvertedData> aggregatedModelItems)
        {
            int batchOrder = Interlocked.Increment(ref _batchOrder);
            int concurrentThreads = Interlocked.Increment(ref _concurrentThreads);            
             _logger.LogInformation(
                 "Start Persisting batch {batchNumber}. Concurrent persisting tasks {concurrentThreads}"
                 , batchOrder
                 , concurrentThreads
                 );

            //await Task.Delay(10000);                 
            aggregatedModelItems ??= new List<ConvertedData>();
            int itemCount = aggregatedModelItems.Count;             
            ImportResult result = new ImportResult()
            {
                Batch = batchOrder,
                HasError = true,
                Data = aggregatedModelItems
            };
            
            int answersCount = itemCount;
            IEnumerable<Category> categories;
            IEnumerable<Question> questions;
            IEnumerable<Answer> answers;

            if (aggregatedModelItems.Count > 0)
            {
                //Remove duplicated items
                categories = aggregatedModelItems.Select<ConvertedData, Category>(converted => converted.Category).DistinctBy(x => x.Id);
                questions = aggregatedModelItems.Select<ConvertedData, Question>(converted => converted.Question).DistinctBy(x => x.Id);
                
                //var aaa = aggregatedModelItems.Select<ConvertedData, Answer>(converted => converted.Answer).GroupBy(ans => ans.Id).Where(grp => grp.Count() > 1).Select(grp => grp);
                //Answers should be always different but ...
                answers = aggregatedModelItems.Select<ConvertedData, Answer>(converted => converted.Answer).DistinctBy(x => x.Id);
                answersCount = answers.Count();
                   
                _logger.LogDebug(
                    "Analyzed {itemCount} elements. {answers.Count()} distinct answers, {categories.Count()} distinct categories and {questions.Count()} distinct questions on Thread {Thread.CurrentThread.ManagedThreadId}"
                    , itemCount
                    , answersCount
                    , categories.Count()
                    , questions.Count()
                    , Thread.CurrentThread.ManagedThreadId
                );                              
                
                try
                {
                    using (SqlConnection destinationConnection = new SqlConnection(_importSettings.DbConnectionString))
                    {
                        destinationConnection.Open();

                        using (SqlTransaction transaction =
                                destinationConnection.BeginTransaction())
                        {
                            try
                            {                               
                                //Recreate Temp Tables
                                using (SqlCommand cmd = new SqlCommand(sqlDropAndRecreateTempTables, destinationConnection, transaction))
                                {
                                    cmd.CommandTimeout = 600;    
                                    await cmd.ExecuteNonQueryAsync();
                                }
                                //Fill Temp Tables
                                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(
                                           destinationConnection, SqlBulkCopyOptions.KeepIdentity,
                                           transaction))
                                {
                                    bulkCopy.BatchSize = _importSettings.BatchSize;
                                    bulkCopy.BulkCopyTimeout = 600;

                                    //Insert Categories
                                    bulkCopy.DestinationTableName = "#Categories";
                                    bulkCopy.ColumnMappings.Add("Id", "Id");
                                    using (IDataReader reader = categories.AsDataReaderOfObjects())
                                    {
                                        try
                                        {
                                            await bulkCopy.WriteToServerAsync(reader);
                                        }
                                        finally
                                        {
                                            reader.Close();
                                        }
                                    }

                                    //Insert Questions
                                    bulkCopy.DestinationTableName = "#Questions";
                                    bulkCopy.ColumnMappings.Clear();
                                    bulkCopy.ColumnMappings.Add("Id", "Id");
                                    bulkCopy.ColumnMappings.Add("CategoryId", "CategoryId");
                                    bulkCopy.ColumnMappings.Add("Text", "Text");
                                    using (IDataReader reader = questions.AsDataReaderOfObjects())
                                    {
                                        await bulkCopy.WriteToServerAsync(reader);
                                    }

                                    //Insert Answers
                                    bulkCopy.DestinationTableName = "#Answers";
                                    bulkCopy.ColumnMappings.Clear();
                                    bulkCopy.ColumnMappings.Add("Id", "Id");
                                    bulkCopy.ColumnMappings.Add("QuestionId", "QuestionId");
                                    bulkCopy.ColumnMappings.Add("Text", "Text");
                                    bulkCopy.ColumnMappings.Add("Timestamp", "Timestamp");
                                    using (IDataReader reader = answers.AsDataReaderOfObjects())
                                    {
                                        await bulkCopy.WriteToServerAsync(reader);
                                    }
                                }

                                //Add indexes on temp tables
                                using (SqlCommand cmd = new SqlCommand(sqlCreateIndexes, destinationConnection, transaction))
                                {
                                    cmd.CommandTimeout = 600;
                                    await cmd.ExecuteNonQueryAsync();
                                }

                                //Insert new records and drop temp tables
                                using (SqlCommand cmd = new SqlCommand(sqlInsertNewRecords, destinationConnection, transaction))
                                {
                                    cmd.CommandTimeout = 1200;
                                    await cmd.ExecuteNonQueryAsync();
                                }
                                //_logger.LogDebug("Committing Transaction");
                                await transaction.CommitAsync();
                                result.HasError = false;
                            }
                            catch
                            {                                                                                                                                                              
                                if (transaction != null )
                                    transaction.Rollback();                                
                                throw;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.OccuredException = ex;
                }
                finally
                {
                    Interlocked.Decrement(ref _concurrentThreads);
                }

                Interlocked.Add(ref _persisted, answersCount);
                Interlocked.Add(ref _total, itemCount);                
            }
            _logger.LogInformation("Items sent to database till now: distinct in block {_persisted} / total {_total}", _persisted, _total);
            return result;
        }
    }
}


