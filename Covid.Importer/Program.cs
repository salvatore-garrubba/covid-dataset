using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.DependencyInjection;

using AutoMapper;
using Covid.Importer.Services;
using System.Threading;
using Covid.Importer.Persistence;
using Covid.Importer.Domain.Models;
using Microsoft.Extensions.Options;
using Covid.Data.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
//using AutoMapper.Extensions;


namespace Covid.Importer
{
    class Program
    {
        async static Task Main(string[] args)
        {            
            
            var builder = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            var importSettingsSection = config.GetSection("ImportSettings");

            var localImportSettings = new ImportSettings();
            importSettingsSection.Bind(localImportSettings);

            //setup Logging and DI
            var serviceCollection = new ServiceCollection()
                .AddLogging(builder =>
                builder.AddConfiguration(config.GetSection("Logging"))
                .AddConsole()
                .AddDebug()
                )
                .Configure<ImportSettings>(config.GetSection("ImportSettings"))
                .AddAutoMapper(typeof(Program))
                .AddSingleton<IDecoderService, DecoderService>()
                .AddSingleton<IImportService, ImportService>()
                .AddSingleton<IInputReaderService, InputReaderService>()
                .AddSingleton<IDBPersister, DBPersister>()
                .AddSingleton<IErrorPersister, FileErrorPersister>();
            if (localImportSettings.RecreateDataBase)
            {    
                serviceCollection.AddDbContext<CovidContext>(options =>
                    options.UseSqlServer(localImportSettings.DbConnectionString)
                 );
            }
            var serviceProvider = serviceCollection.BuildServiceProvider();

            if(localImportSettings.RecreateDataBase)
            {
                var dbContext = serviceProvider.GetRequiredService<CovidContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }

            var importService = serviceProvider.GetRequiredService<IImportService>();
            var cts = new CancellationTokenSource();
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                var importProcess = Task.Run(async () =>
                {
                    await importService.StartImportAsync(cts.Token);
                });

                // await Task.Delay(5000);   
                // cts.Cancel();            
                await importProcess;
                sw.Stop();

                //just few waiting to hopefully see following console lines at the end
                await Task.Delay(1000);

                Console.WriteLine($"{sw.Elapsed.TotalSeconds} seconds elapsed");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                string cm = ex.Message;
            }

        }
    }
}
