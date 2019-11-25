using Microsoft.Extensions.DependencyInjection;
using Squadio.Common.Extensions;
using Squadio.EmailSender.RabbitMessageHandler;

namespace Squadio.EmailSender
{
    public class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.Add<IRabbitMessageHandler, RabbitMessageHandler.Implementation.RabbitMessageHandler>(ServiceLifetime.Transient);
        }
    }
}