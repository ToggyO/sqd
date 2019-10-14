using System.Linq;
using Magora.Passwords;
using Mapper;
using Microsoft.Extensions.DependencyInjection;
using Squadio.BLL.Factories;
using Squadio.BLL.Factories.Implementation;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Providers.Companies.Implementation;
using Squadio.BLL.Providers.SignUp;
using Squadio.BLL.Providers.SignUp.Implementation;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Providers.Users.Implementation;
using Squadio.BLL.Services.Companies;
using Squadio.BLL.Services.Companies.Implementation;
using Squadio.BLL.Services.Email;
using Squadio.BLL.Services.Email.Implementations;
using Squadio.BLL.Services.Email.Sender;
using Squadio.BLL.Services.Email.Sender.Implementation;
using Squadio.BLL.Services.SignUp;
using Squadio.BLL.Services.SignUp.Implementation;
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

            LoadEmailServices(services);
            
            services.AddTransient<IPasswordService, PasswordService>();
            
            services.Add<IUsersProvider, UsersProvider>(serviceLifetime);
            services.Add<IUsersService, UsersService>(serviceLifetime);
            
            services.Add<ISignUpProvider, SignUpProvider>(serviceLifetime);
            services.Add<ISignUpService, SignUpService>(serviceLifetime);
            
            services.Add<ITokensService, TokensService>(serviceLifetime);
            
            services.Add<ICompaniesProvider, CompaniesProvider>(serviceLifetime);
            services.Add<ICompaniesService, CompaniesService>(serviceLifetime);
            
            services.Add<ITokensFactory, TokensFactory>(serviceLifetime);
            services.AddMapper();
        }
        
        private static void LoadEmailServices(IServiceCollection services)
        {
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IEmailService, EmailService>();
            
            foreach (var implementationType in typeof(DependencyInjectionModule).Assembly.GetTypes().Where(t =>
            {
                if (t.IsClass)
                    return !t.IsAbstract;
                return false;
            }))
            {
                foreach (var type in implementationType.GetInterfaces())
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEmailService<>))
                    {
                        var serviceType = typeof(IEmailService<>).MakeGenericType(type.GetGenericArguments());
                        services.AddScoped(serviceType, implementationType);
                    }
                }
            }
        }
    }
}
