using Microsoft.Extensions.DependencyInjection;
using Squadio.DAL.Repository.Users;
using Squadio.DAL.Repository.Users.Implementation;
using Squadio.Common.Extensions;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.Companies.Implementation;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.CompaniesUsers.Implementation;

namespace Squadio.DAL
{
    public class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.Add<IUsersRepository, UsersRepository>(serviceLifetime);
            services.Add<ICompaniesRepository, CompaniesRepository>(serviceLifetime);;
            services.Add<ICompaniesUsersRepository, CompaniesUsersRepository>(serviceLifetime);
        }
    }
}
