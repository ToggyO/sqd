using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.SystemConsole.Themes;
using Squadio.Common.Settings;

namespace Squadio.API.Extensions
{
    public static class AddSerilogExtension
    {
        public static IServiceCollection AddSerilog(this IServiceCollection services, DbSettings dbSettings)
        {
            try
            {
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
                        restrictedToMinimumLevel: LogEventLevel.Error, 
                        schemaName: "public", 
                        needAutoCreateTable: true))
                    .WriteTo.Async(x => x.Console(
                        theme: SystemConsoleTheme.Literate,
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message}{NewLine}{Exception}"))
                    .CreateLogger();
  
                services.AddSingleton(Log.Logger);
 
                return services;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public static IHostApplicationLifetime SerilogRegisterCloseAndFlush(this IHostApplicationLifetime lifetime)
        {
            lifetime.ApplicationStopping.Register(Log.CloseAndFlush);
            return lifetime;
        }
    }
}