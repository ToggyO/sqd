using Mapper;
using Microsoft.Extensions.DependencyInjection;

namespace Squadio.DTO
{
    public class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services)
        {
            services.AddMapper<DependencyInjectionModule>();
        }
    }
}