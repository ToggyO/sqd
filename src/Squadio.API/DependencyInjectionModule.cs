using Mapper;
using Microsoft.Extensions.DependencyInjection;
using Squadio.API.Handlers.Auth;
using Squadio.API.Handlers.Auth.Implementation;
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

            services.Add<IUsersHandler, UsersHandler>(serviceLifetime);
            services.Add<IAuthHandler, AuthHandler>(serviceLifetime);
        }
    }
}
