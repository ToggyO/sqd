
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

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
                    var startup = new Startup(hostContext.HostingEnvironment);
                    startup.ConfigureServices(services);
                });
    }
}