using System;
using Microsoft.Extensions.DependencyInjection;

namespace Squadio.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void Add<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime)
            where TService : class where TImplementation : class, TService
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<TService, TImplementation>();
                    break;

                case ServiceLifetime.Scoped:
                    services.AddScoped<TService, TImplementation>();
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient<TService, TImplementation>();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null);
            }
        }

        public static void Add<TService>(this IServiceCollection services, ServiceLifetime serviceLifetime)
            where TService : class
        {
            services.Add<TService, TService>(serviceLifetime);
        }
    }
}
