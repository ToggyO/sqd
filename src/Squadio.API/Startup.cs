using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.SystemConsole.Themes;
using Squadio.DAL;
using Squadio.API.Extensions;
using Squadio.API.Filters;
using Squadio.BLL.Services.WebSocket;
using Squadio.Common.Models.Rabbit;
using Squadio.Common.Settings;

namespace Squadio.API
{
    public class Startup
    {
        private const string MyAllowSquadioOrigins = "_myAllowSquadioOrigins";
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
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSquadioOrigins,
                    builder =>
                    {
                        builder.WithOrigins(
                                "http://localhost:3010",
                                "http://localhost:5005",
                                "https://squad.api.magora.work",
                                "https://squad.magora.work")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            
            services.AddSignalR();

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
            services.Configure<RabbitConnectionModel>(Configuration.GetSection("RabbitConnection"));
            services.Configure<ApiSettings>(Configuration.GetSection("AppSettings:APISettings"));
            services.Configure<FileTemplateUrlModel>(Configuration.GetSection("FileTemplateUrl"));
            services.Configure<FileRootDirectoryModel>(Configuration.GetSection("FileRootDirectory"));
            services.Configure<CropSizesModel>(Configuration.GetSection("CropSizes"));

            services.AddSerilog(dbSettings);

            services.AddDbContext<SquadioDbContext>(builder =>
                    builder
                        .EnableSensitiveDataLogging()
                        .UseNpgsql(dbSettings.PostgresConnectionString,
                            optionsBuilder =>
                                optionsBuilder.MigrationsAssembly(typeof(SquadioDbContext).Assembly.FullName)));

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Squad.io API", Version = "v1" });
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
            
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
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
                        
                        // To allow return custom response for expired token
                        ValidateLifetime = false
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            
                            if (!string.IsNullOrEmpty(accessToken) 
                                && path.StartsWithSegments("/api/ws"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            
            services.AddAuthorization();

            DependencyInjectionModule.Load(services);
        }

        public void Configure(IApplicationBuilder app
            , IWebHostEnvironment env
            , IHostApplicationLifetime hostLifetime
            , ILogger<Startup> logger)
        {
            hostLifetime.SerilogRegisterCloseAndFlush();
            
            logger.LogInformation("Enter Configure");
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            
            logger.LogInformation("Routing");
            app.UseRouting();

            logger.LogInformation("Auth");
            app.UseAuthentication();
            app.UseAuthorization();
            
            logger.LogInformation("Middleware");
            app.UseMiddleware(typeof(ExceptionMiddleware));

            logger.LogInformation("Cors");
            app.UseCors(MyAllowSquadioOrigins);
            
            logger.LogInformation("Static files");
            app.UseStaticFiles();

            logger.LogInformation("Swagger");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Squad.io API V1");
            });

            logger.LogInformation("EnsureMigrationOfContext");
            app.EnsureMigrationOfContext<SquadioDbContext>();

            logger.LogInformation("Endpoints");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHubService>("api/ws/chat");
                endpoints.MapHub<CommonHubService>("api/ws");
            });
            logger.LogInformation("Exit Configure");
        }
    }
}
