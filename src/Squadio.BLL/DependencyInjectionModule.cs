using Magora.Passwords;
using Mapper;
using Microsoft.Extensions.DependencyInjection;
using Squadio.BLL.Factories;
using Squadio.BLL.Factories.Implementation;
using Squadio.BLL.Providers.Admins;
using Squadio.BLL.Providers.Admins.Implementation;
using Squadio.BLL.Providers.Codes;
using Squadio.BLL.Providers.Codes.Implementation;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Providers.Companies.Implementation;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Providers.Projects.Implementation;
using Squadio.BLL.Providers.Resources;
using Squadio.BLL.Providers.Resources.Implementation;
using Squadio.BLL.Providers.SignUp;
using Squadio.BLL.Providers.SignUp.Implementation;
using Squadio.BLL.Providers.Teams;
using Squadio.BLL.Providers.Teams.Implementation;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Providers.Users.Implementation;
using Squadio.BLL.Providers.WebSocket.BaseHubProvider;
using Squadio.BLL.Providers.WebSocket.Sidebar;
using Squadio.BLL.Services.Admins;
using Squadio.BLL.Services.Admins.Implementation;
using Squadio.BLL.Services.Companies;
using Squadio.BLL.Services.Companies.Implementation;
using Squadio.BLL.Services.ConfirmEmail;
using Squadio.BLL.Services.ConfirmEmail.Implementation;
using Squadio.BLL.Services.Files;
using Squadio.BLL.Services.Files.Implementation;
using Squadio.BLL.Services.Projects;
using Squadio.BLL.Services.Projects.Implementation;
using Squadio.BLL.Services.Rabbit;
using Squadio.BLL.Services.Rabbit.Implementations;
using Squadio.BLL.Services.Rabbit.Publisher;
using Squadio.BLL.Services.Rabbit.Publisher.Implementation;
using Squadio.BLL.Services.Resources;
using Squadio.BLL.Services.Resources.Implementation;
using Squadio.BLL.Services.SignUp;
using Squadio.BLL.Services.SignUp.Implementation;
using Squadio.BLL.Services.Teams;
using Squadio.BLL.Services.Teams.Implementation;
using Squadio.BLL.Services.Tokens;
using Squadio.BLL.Services.Tokens.Implementation;
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

            LoadRabbitServices(services);
            
            services.AddTransient<IPasswordService, PasswordService>();
            
            services.Add<ICodeProvider, CodeProvider>(serviceLifetime);
            services.Add<IConfirmEmailService, ConfirmEmailService>(serviceLifetime);
            
            services.Add<IUsersProvider, UsersProvider>(serviceLifetime);
            services.Add<IUsersService, UsersService>(serviceLifetime);
            
            services.Add<ISignUpProvider, SignUpProvider>(serviceLifetime);
            services.Add<ISignUpService, SignUpService>(serviceLifetime);
            
            services.Add<ITokensService, TokensService>(serviceLifetime);
            
            services.Add<ICompaniesProvider, CompaniesProvider>(serviceLifetime);
            services.Add<ICompaniesService, CompaniesService>(serviceLifetime);
            services.Add<ICompanyInvitesService, CompanyInvitesService>(serviceLifetime);
            
            services.Add<ITeamsProvider, TeamsProvider>(serviceLifetime);
            services.Add<ITeamsService, TeamsService>(serviceLifetime);
            services.Add<ITeamInvitesService, TeamInvitesService>(serviceLifetime);
            
            services.Add<IProjectsProvider, ProjectsProvider>(serviceLifetime);
            services.Add<IProjectsService, ProjectsService>(serviceLifetime);
            services.Add<IProjectInvitesService, ProjectInvitesService>(serviceLifetime);
            
            services.Add<IAdminsProvider, AdminsProvider>(serviceLifetime);
            services.Add<IAdminsService, AdminsService>(serviceLifetime);
            
            services.Add<IFilesService, FilesService>(serviceLifetime);
            services.Add<IResourcesProvider, ResourcesProvider>(serviceLifetime);
            services.Add<IResourcesService, ResourcesService>(serviceLifetime);
            
            services.Add<IBaseHubProvider, BaseHubProvider>(serviceLifetime);
            services.Add<ISidebarHubProvider, SidebarHubProvider>(serviceLifetime);
            
            services.Add<ITokensFactory, TokensFactory>(serviceLifetime);
            services.AddMapper();
        }
        
        private static void LoadRabbitServices(IServiceCollection services)
        {
            services.AddScoped<IRabbitPublisher, RabbitPublisher>();
            services.AddScoped<IRabbitService, RabbitService>();
        }
    }
}
