using Microsoft.Extensions.DependencyInjection;
using Squadio.API.Filters;
using Squadio.API.Handlers.Admins;
using Squadio.API.Handlers.Admins.Implementation;
using Squadio.API.Handlers.Auth;
using Squadio.API.Handlers.Auth.Implementation;
using Squadio.API.Handlers.Companies;
using Squadio.API.Handlers.Companies.Implementation;
using Squadio.API.Handlers.Invites;
using Squadio.API.Handlers.Invites.Implementation;
using Squadio.API.Handlers.Projects;
using Squadio.API.Handlers.Projects.Implementation;
using Squadio.API.Handlers.Resources;
using Squadio.API.Handlers.Resources.Implementation;
using Squadio.API.Handlers.SignUp;
using Squadio.API.Handlers.SignUp.Implementation;
using Squadio.API.Handlers.Teams;
using Squadio.API.Handlers.Teams.Implementation;
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

            services.Add<ISignUpHandler, SignUpHandler>(serviceLifetime);
            services.Add<IUsersHandler, UsersHandler>(serviceLifetime);
            services.Add<IAuthHandler, AuthHandler>(serviceLifetime);
            services.Add<ICompaniesHandler, CompaniesHandler>(serviceLifetime);
            services.Add<ITeamsHandler, TeamsHandler>(serviceLifetime);
            services.Add<IProjectsHandler, ProjectsHandler>(serviceLifetime);
            services.Add<IInvitesHandler, InvitesHandler>(serviceLifetime);
            services.Add<IAdminsHandler, AdminsHandler>(serviceLifetime);
            services.Add<IFilesHandler, FilesHandler>(serviceLifetime);
            
            services.Add<AuthorizationFilter>(serviceLifetime);
        }
    }
}
