using Magora.Passwords;
using Microsoft.Extensions.DependencyInjection;
using Squadio.BLL.Factories;
using Squadio.BLL.Factories.Implementations;
using Squadio.BLL.Providers.Admin;
using Squadio.BLL.Providers.Admin.Implementations;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Providers.Companies.Implementations;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Providers.Projects.Implementations;
using Squadio.BLL.Providers.Resources;
using Squadio.BLL.Providers.Resources.Implementations;
using Squadio.BLL.Providers.SignUp;
using Squadio.BLL.Providers.SignUp.Implementations;
using Squadio.BLL.Providers.Teams;
using Squadio.BLL.Providers.Teams.Implementations;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Providers.Users.Implementations;
using Squadio.BLL.Services.Admin;
using Squadio.BLL.Services.Admin.Implementations;
using Squadio.BLL.Services.Companies;
using Squadio.BLL.Services.Companies.Implementations;
using Squadio.BLL.Services.ConfirmEmail;
using Squadio.BLL.Services.ConfirmEmail.Implementations;
using Squadio.BLL.Services.Files;
using Squadio.BLL.Services.Files.Implementations;
using Squadio.BLL.Services.Invites;
using Squadio.BLL.Services.Invites.Implementations;
using Squadio.BLL.Services.Membership;
using Squadio.BLL.Services.Membership.Implementations;
using Squadio.BLL.Services.Notifications.Emails;
using Squadio.BLL.Services.Notifications.Emails.Implementations;
using Squadio.BLL.Services.Projects;
using Squadio.BLL.Services.Projects.Implementations;
using Squadio.BLL.Services.Resources;
using Squadio.BLL.Services.Resources.Implementations;
using Squadio.BLL.Services.SignUp;
using Squadio.BLL.Services.SignUp.Implementations;
using Squadio.BLL.Services.Teams;
using Squadio.BLL.Services.Teams.Implementations;
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
            
            services.Add<IInvitesService, InvitesService>(serviceLifetime);
            
            services.Add<IMembershipService, MembershipService>(serviceLifetime);
            
            services.Add<ICompaniesProvider, CompaniesProvider>(serviceLifetime);
            services.Add<ICompaniesService, CompaniesService>(serviceLifetime);
            
            services.Add<ITeamsProvider, TeamsProvider>(serviceLifetime);
            services.Add<ITeamsService, TeamsService>(serviceLifetime);
            
            services.Add<IProjectsProvider, ProjectsProvider>(serviceLifetime);
            services.Add<IProjectsService, ProjectsService>(serviceLifetime);
            
            services.Add<ITokensService, TokensService>(serviceLifetime);
            
            services.Add<ISignUpService, SignUpService>(serviceLifetime);
            services.Add<ISignUpProvider, SignUpProvider>(serviceLifetime);
            
            services.Add<IAdminsProvider, AdminsProvider>(serviceLifetime);
            services.Add<IAdminsService, AdminsService>(serviceLifetime);
            
            services.Add<IFilesService, FilesService>(serviceLifetime);
            services.Add<IResourcesProvider, ResourcesProvider>(serviceLifetime);
            services.Add<IResourcesService, ResourcesService>(serviceLifetime);
            
            services.Add<ITokensFactory, TokensFactory>(serviceLifetime);
        }
    }
}
