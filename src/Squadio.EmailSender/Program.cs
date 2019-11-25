using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Squadio.EmailSender
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) => 
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(x =>
                { 
                    x.ClearProviders();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    hostContext.HostingEnvironment.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var startup = new Startup(hostContext.HostingEnvironment);
                    startup.ConfigureServices(services);
                });
    }
}