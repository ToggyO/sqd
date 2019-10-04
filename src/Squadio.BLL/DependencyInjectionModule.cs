using System.Linq;
using Mapper;
using Microsoft.Extensions.DependencyInjection;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Providers.Users.Implementation;
using Squadio.BLL.Services.Email.Services;
using Squadio.BLL.Services.Users;
using Squadio.BLL.Services.Users.Implementation;
using Squadio.Common.Extensions;

namespace Squadio.BLL
{
    public class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            DAL.DependencyInjectionModule.Load(services);
            DTO.DependencyInjectionModule.Load(services);
            
            LoadEmailServices(services);
            
            services.Add<IUserProvider, UserProvider>(serviceLifetime);
            services.Add<IUserService, UserService>(serviceLifetime);
            services.AddMapper();
        }
        
        private static void LoadEmailServices(IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
            
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
                        services.AddScoped(serviceType, implementationType);
                    }
                }
            }
        }
    }
}
