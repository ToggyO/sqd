using Mapper;
using Microsoft.Extensions.DependencyInjection;
using Squadio.API.Handlers.Users;
using Squadio.API.Handlers.Users.Implementation;
using Squadio.Common.Extensions;

namespace Squadio.API
{
    public class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            BLL.DependencyInjectionModule.Load(services);

            services.Add<IUserHandler, UserHandler>(serviceLifetime);
        }
    }
}
