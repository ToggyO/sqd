using Magora.Passwords;
using Microsoft.Extensions.DependencyInjection;
using Squadio.BLL.Factories;
using Squadio.BLL.Factories.Implementations;
using Squadio.BLL.Providers.Admins;
using Squadio.BLL.Providers.Admins.Implementations;
using Squadio.BLL.Providers.Resources;
using Squadio.BLL.Providers.Resources.Implementations;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Providers.Users.Implementations;
using Squadio.BLL.Services.Admins;
using Squadio.BLL.Services.Admins.Implementations;
using Squadio.BLL.Services.ConfirmEmail;
using Squadio.BLL.Services.ConfirmEmail.Implementations;
using Squadio.BLL.Services.Files;
using Squadio.BLL.Services.Files.Implementations;
using Squadio.BLL.Services.Notifications.Emails;
using Squadio.BLL.Services.Notifications.Emails.Implementations;
using Squadio.BLL.Services.Resources;
using Squadio.BLL.Services.Resources.Implementations;
using Squadio.BLL.Services.SignUp;
using Squadio.BLL.Services.SignUp.Implementations;
using Squadio.BLL.Services.Tokens;
using Squadio.BLL.Services.Tokens.Implementations;
using Squadio.BLL.Services.Users;
using Squadio.BLL.Services.Users.Implementations;
using Squadio.Common.Extensions;

namespace Squadio.BLL
{
    public class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            DAL.DependencyInjectionModule.Load(services);
            DTO.DependencyInjectionModule.Load(services);
            
            services.AddTransient<IPasswordService, PasswordService>();
            
            services.Add<IConfirmEmailService, ConfirmEmailService>(serviceLifetime);
            services.Add<IEmailNotificationsService, EmailNotificationsService>(serviceLifetime);
            
            services.Add<IUsersProvider, UsersProvider>(serviceLifetime);
            services.Add<IUsersService, UsersService>(serviceLifetime);
            
            services.Add<ITokensService, TokensService>(serviceLifetime);
            services.Add<ISignUpService, SignUpService>(serviceLifetime);
            
            services.Add<IAdminsProvider, AdminsProvider>(serviceLifetime);
            services.Add<IAdminsService, AdminsService>(serviceLifetime);
            
            services.Add<IFilesService, FilesService>(serviceLifetime);
            services.Add<IResourcesProvider, ResourcesProvider>(serviceLifetime);
            services.Add<IResourcesService, ResourcesService>(serviceLifetime);
            
            services.Add<ITokensFactory, TokensFactory>(serviceLifetime);
        }
    }
}
