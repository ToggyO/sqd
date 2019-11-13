using Microsoft.Extensions.DependencyInjection;
using Squadio.DAL.Repository.Users;
using Squadio.DAL.Repository.Users.Implementation;
using Squadio.Common.Extensions;
using Squadio.DAL.Repository.Admins;
using Squadio.DAL.Repository.Admins.Implementation;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.ChangePassword.Implementation;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.Companies.Implementation;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.CompaniesUsers.Implementation;
using Squadio.DAL.Repository.ConfirmEmail;
using Squadio.DAL.Repository.ConfirmEmail.Implementation;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.Invites.Implementation;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.Projects.Implementation;
using Squadio.DAL.Repository.ProjectsUsers;
using Squadio.DAL.Repository.ProjectsUsers.Implementation;
using Squadio.DAL.Repository.SignUp;
using Squadio.DAL.Repository.SignUp.Implementation;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.Teams.Implementation;
using Squadio.DAL.Repository.TeamsUsers;
using Squadio.DAL.Repository.TeamsUsers.Implementation;

namespace Squadio.DAL
{
    public class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.Add<IUsersRepository, UsersRepository>(serviceLifetime);
            services.Add<IChangePasswordRequestRepository, ChangePasswordRequestRepository>(serviceLifetime);
            services.Add<IConfirmEmailRequestRepository, ConfirmEmailRequestRepository>(serviceLifetime);
            
            services.Add<ICompaniesRepository, CompaniesRepository>(serviceLifetime);;
            services.Add<ICompaniesUsersRepository, CompaniesUsersRepository>(serviceLifetime);
            
            services.Add<ITeamsRepository, TeamsRepository>(serviceLifetime);
            services.Add<ITeamsUsersRepository, TeamsUsersRepository>(serviceLifetime);
            
            services.Add<IProjectsRepository, ProjectsRepository>(serviceLifetime);
            services.Add<IProjectsUsersRepository, ProjectsUsersRepository>(serviceLifetime);
            
            services.Add<IInvitesRepository, InvitesRepository>(serviceLifetime);
            
            services.Add<ISignUpRepository, SignUpRepository>(serviceLifetime);
            
            services.Add<IAdminsRepository, AdminsRepository>(serviceLifetime);
        }
    }
}
