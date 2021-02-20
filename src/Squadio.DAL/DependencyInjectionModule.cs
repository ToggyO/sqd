using Microsoft.Extensions.DependencyInjection;
using Squadio.DAL.Repository.Users;
using Squadio.DAL.Repository.Users.Implementation;
using Squadio.Common.Extensions;
using Squadio.DAL.Repository.Admins;
using Squadio.DAL.Repository.Admins.Implementation;
using Squadio.DAL.Repository.ChangeEmail;
using Squadio.DAL.Repository.ChangeEmail.Implementation;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.ChangePassword.Implementation;
using Squadio.DAL.Repository.ConfirmEmail;
using Squadio.DAL.Repository.ConfirmEmail.Implementation;
using Squadio.DAL.Repository.Resources;
using Squadio.DAL.Repository.Resources.Implementation;

namespace Squadio.DAL
{
    public class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.Add<IUsersRepository, UsersRepository>(serviceLifetime);
            services.Add<IConfirmEmailRequestRepository, ConfirmEmailRequestRepository>(serviceLifetime);
            services.Add<IChangePasswordRequestRepository, ChangePasswordRequestRepository>(serviceLifetime);
            services.Add<IChangeEmailRequestRepository, ChangeEmailRequestRepository>(serviceLifetime);
            
            services.Add<IAdminsRepository, AdminsRepository>(serviceLifetime);
            
            services.Add<IResourcesRepository, ResourcesRepository>(serviceLifetime);
        }
    }
}
