using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Squadio.Common.Extensions;
using Squadio.EmailSender.EmailService;
using Squadio.EmailSender.EmailService.Sender;
using Squadio.EmailSender.RabbitMessageHandler;

namespace Squadio.EmailSender
{
    public class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            LoadEmailServices(services);
            services.Add<IRabbitMessageHandler, RabbitMessageHandler.Implementation.RabbitMessageHandler>(ServiceLifetime.Transient);
        }
        
        private static void LoadEmailServices(IServiceCollection services)
        {
            services.AddTransient<IEmailSender, EmailService.Sender.Implementation.EmailSender>();
            services.AddTransient<IEmailService, EmailService.Implementations.EmailService>();
            
            foreach (var implementationType in typeof(DependencyInjectionModule).Assembly.GetTypes().Where(t =>
            {
                if (t.IsClass)
                    return !t.IsAbstract;
                return false;
            }))
            {
                foreach (var type in implementationType.GetInterfaces())
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEmailService<>))
                    {
                        var serviceType = typeof(IEmailService<>).MakeGenericType(type.GetGenericArguments());
                        services.AddTransient(serviceType, implementationType);
                    }
                }
            }
        }
    }
}