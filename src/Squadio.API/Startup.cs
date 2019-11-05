using System;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Squadio.DAL;
using Squadio.API.Extensions;
using Squadio.API.Filters;
using Squadio.Common.Settings;

namespace Squadio.API
{
    public class Startup
    {
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
            services.AddCors();

            services.AddMvcCore(options =>
                {
                    options.Filters.Add(typeof(ValidateModelAttribute));
                    options.Filters.Add(typeof(StatusCodeFilter));
                })
                .AddApiExplorer()
                .AddFluentValidation(configuration =>
                    configuration.RegisterValidatorsFromAssemblyContaining<DTO.DependencyInjectionModule>());;

            var apiSettings = Configuration.GetSection("AppSettings:APISettings").Get<ApiSettings>();

            services.AddMemoryCache();
            
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            var dbSettings = new DbSettings
            {
                DB_HOST = Configuration.GetSection("DB_HOST").Value,
                DB_PORT = Configuration.GetSection("DB_PORT").Value,
                DB_USER = Configuration.GetSection("DB_USER").Value,
                DB_NAME = Configuration.GetSection("DB_NAME").Value,
                DB_PASSWORD = Configuration.GetSection("DB_PASSWORD").Value
            };

            services.Configure<GoogleSettings>(Configuration.GetSection("GoogleOAuth"));
            
            services.Configure<ApiSettings>(Configuration.GetSection("APISettings"));
            services.Configure<EmailSettingsModel>(Configuration.GetSection("EmailSettings"));
            services.Configure<StaticUrlsSettingsModel>(Configuration.GetSection("StaticUrls"));
            
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                if (Configuration.GetSection("ASPNETCORE_ENVIRONMENT").Value != "Uat")
                    builder.AddConsole();
                builder.AddDebug();
                builder.AddEventSourceLogger();
            });

            services.AddDbContext<SquadioDbContext>(builder =>
                    builder
                        .EnableSensitiveDataLogging()
                        .UseNpgsql(dbSettings.PostgresConnectionString,
                            optionsBuilder =>
                                optionsBuilder.MigrationsAssembly(typeof(SquadioDbContext).Assembly.FullName)));


            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Squad.io API", Version = "v1" });
                //c.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}Squadio.API.xml");
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Please insert JWT with Bearer into field",
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[0] 
                    }
                });
                options.DescribeAllEnumsAsStrings();
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    //options.RequireHttpsMetadata = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        
                        ValidateAudience = true,
                        ValidAudience = apiSettings.AUDIENCE,
                        
                        ValidateIssuer = true,
                        ValidIssuer = apiSettings.ISSUER,
                        
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiSettings.PublicKey)),
                        ValidateIssuerSigningKey = true,
                        
                        ValidateLifetime = true
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
            
            services.AddAuthorization();

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
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseMiddleware(typeof(ExceptionMiddleware));

            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Squad.io API V1");
            });

            app.EnsureMigrationOfContext<SquadioDbContext>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
