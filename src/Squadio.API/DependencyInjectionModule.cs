﻿using Microsoft.Extensions.DependencyInjection;
using Squadio.API.Filters;
using Squadio.API.Handlers.Admins;
using Squadio.API.Handlers.Admins.Implementations;
using Squadio.API.Handlers.Auth;
using Squadio.API.Handlers.Auth.Implementations;
using Squadio.API.Handlers.Resources;
using Squadio.API.Handlers.Resources.Implementations;
using Squadio.API.Handlers.SignUp;
using Squadio.API.Handlers.SignUp.Implementations;
using Squadio.API.Handlers.Users;
using Squadio.API.Handlers.Users.Implementations;
using Squadio.Common.Extensions;

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
            
            services.Add<AuthorizationFilter>(serviceLifetime);
        }
    }
}
