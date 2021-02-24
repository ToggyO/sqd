using Magora.Passwords;
using Microsoft.Extensions.DependencyInjection;
using Squadio.BLL.Factories;
using Squadio.BLL.Factories.Implementation;
using Squadio.BLL.Providers.Admins;
using Squadio.BLL.Providers.Admins.Implementations;
using Squadio.BLL.Providers.Codes;
using Squadio.BLL.Providers.Codes.Implementation;
using Squadio.BLL.Providers.Resources;
using Squadio.BLL.Providers.Resources.Implementation;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Providers.Users.Implementations;
using Squadio.BLL.Services.Admins;
using Squadio.BLL.Services.Admins.Implementations;
using Squadio.BLL.Services.ConfirmEmail;
using Squadio.BLL.Services.ConfirmEmail.Implementations;
using Squadio.BLL.Services.Files;
using Squadio.BLL.Services.Files.Implementation;
using Squadio.BLL.Services.Rabbit;
using Squadio.BLL.Services.Rabbit.Implementations;
using Squadio.BLL.Services.Rabbit.Publisher;
using Squadio.BLL.Services.Rabbit.Publisher.Implementation;
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

            LoadRabbitServices(services);
            
            services.AddTransient<IPasswordService, PasswordService>();
            
            services.Add<ICodeProvider, CodeProvider>(serviceLifetime);
            services.Add<IConfirmEmailService, ConfirmEmailService>(serviceLifetime);
            
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
        
        private static void LoadRabbitServices(IServiceCollection services)
        {
            services.AddScoped<IRabbitPublisher, RabbitPublisher>();
            services.AddScoped<IRabbitService, RabbitService>();
        }
    }
}
