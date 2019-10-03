using Microsoft.Extensions.DependencyInjection;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Providers.Users.Implementation;
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

            services.Add<IUserProvider, UserProvider>(serviceLifetime);
            services.Add<IUserService, UserService>(serviceLifetime);
        }
    }
}
