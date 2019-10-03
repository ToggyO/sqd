using System;
using System.Text;
using System.Threading.Tasks;

using Squadio.Common.Extensions;
using Squadio.DAL;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;
using Squadio.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Squadio.Common.Settings;

using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Linq;

namespace Squadio.API
{
    public class Startup
    {
        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        private readonly IConfiguration Configuration;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var apiSettings = Configuration.GetSection("AppSettings:APISettings").Get<ApiSettings>();


            services.AddMemoryCache();

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            services.AddDbContextPool<SquadioDbContext>(builder =>
                    builder
                        .EnableSensitiveDataLogging()
                        .UseNpgsql(string.Format(connectionString),
                            optionsBuilder =>
                                optionsBuilder.MigrationsAssembly(typeof(SquadioDbContext).Assembly.FullName)));


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Squad.io API", Version = "v1" });
                c.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}Squadio.API.xml");
                c.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Please insert JWT with Bearer into field",
                        Name = "Authorization",
                        Type = "apiKey"
                    });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", Enumerable.Empty<string>()}
                });
            });
            

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5000")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = apiSettings.ISSUER,
                        ValidAudience = apiSettings.AUDIENCE,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiSettings.PublicKey)),
                        ValidateIssuerSigningKey = true
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddMvc();

            DependencyInjectionModule.Load(services);
        }

        public void Configure(IApplicationBuilder app
            , IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            /*
            app.UseMiddleware(typeof(ExceptionMiddleware));

            app.UseSwagger();*/
            app.UseCors(MyAllowSpecificOrigins);
            app.UseStaticFiles();

            /*app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Anova API V1");

                c.ShowExtensions();
            });*/

            app.EnsureMigrationOfContext<SquadioDbContext>();

            app.UseMvcWithDefaultRoute();
            app.UseRequestLocalization();
        }
    }
}
