using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;
using Squadio.Common.Settings;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.SystemConsole.Themes;

namespace Squadio.EmailSender
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory());
            
            var dbSettings = new DbSettings
            {
                DB_HOST = Configuration.GetSection("DB_HOST").Value,
                DB_PORT = Configuration.GetSection("DB_PORT").Value,
                DB_USER = Configuration.GetSection("DB_USER").Value,
                DB_NAME = Configuration.GetSection("DB_NAME").Value,
                DB_PASSWORD = Configuration.GetSection("DB_PASSWORD").Value
            };
            
            var columnWriters = new Dictionary<string, ColumnWriterBase>
            {
                {"Message", new RenderedMessageColumnWriter() },
                {"MessageTemplate", new MessageTemplateColumnWriter() },
                {"Level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                {"Date", new TimestampColumnWriter() },
                {"Exception", new ExceptionColumnWriter() }
            };

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Async(x => x.PostgreSQL(
                    dbSettings.PostgresConnectionString, 
                    "Logs", 
                    columnWriters, 
                    restrictedToMinimumLevel: LogEventLevel.Warning, 
                    schemaName: "public", 
                    needAutoCreateTable: true))
                .WriteTo.Async(x => x.Console(
                    restrictedToMinimumLevel: LogEventLevel.Verbose,
                    theme: SystemConsoleTheme.Literate,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message}{NewLine}{Exception}"))
                .CreateLogger();

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            
            services.AddHostedService<ListenerRabbitMQ>();
        }
    }
}