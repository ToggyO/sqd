using Microsoft.Extensions.DependencyInjection;
using Squadio.API.Filters;
using Squadio.API.Handlers.Admins;
using Squadio.API.Handlers.Admins.Implementations;
using Squadio.API.Handlers.Auth;
using Squadio.API.Handlers.Auth.Implementations;
using Squadio.API.Handlers.Companies;
using Squadio.API.Handlers.Companies.Implementations;
using Squadio.API.Handlers.Projects;
using Squadio.API.Handlers.Projects.Implementations;
using Squadio.API.Handlers.Resources;
using Squadio.API.Handlers.Resources.Implementations;
using Squadio.API.Handlers.SignUp;
using Squadio.API.Handlers.SignUp.Implementations;
using Squadio.API.Handlers.Teams;
using Squadio.API.Handlers.Teams.Implementations;
using Squadio.API.Handlers.Users;
using Squadio.API.Handlers.Users.Implementations;
using Squadio.Common.Extensions;
using Squadio.Domain.Enums;

namespace Squadio.API
{
    public class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            BLL.DependencyInjectionModule.Load(services);

            services.Add<IUsersHandler, UsersHandler>(serviceLifetime);
            services.Add<IUsersSettingsHandler, UsersSettingsHandler>(serviceLifetime);
            services.Add<IAuthHandler, AuthHandler>(serviceLifetime);
            services.Add<IAdminsHandler, AdminsHandler>(serviceLifetime);
            services.Add<IFilesHandler, FilesHandler>(serviceLifetime);
            services.Add<ISignUpHandler, SignUpHandler>(serviceLifetime);
            services.Add<ICompaniesHandler, CompaniesHandler>(serviceLifetime);
            services.Add<ITeamsHandler, TeamsHandler>(serviceLifetime);
            services.Add<IProjectsHandler, ProjectsHandler>(serviceLifetime);
            
            services.Add<AuthorizationFilter>(serviceLifetime);
            services.Add<UserStatusFilter>(serviceLifetime);
        }
    }
}
