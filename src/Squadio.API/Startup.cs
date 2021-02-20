using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Squadio.DAL;
using Squadio.API.Extensions;
using Squadio.API.Filters;
using Squadio.BLL.Mapping;
using Squadio.Common.Models.Rabbit;
using Squadio.Common.Settings;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Squadio.API
{
    public class Startup
    {
        private const string MyAllowSquadioOrigins = "_myAllowSquadioOrigins";
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureBaseServices(services, ServiceLifetime.Scoped);
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSquadioOrigins,
                    builder =>
                    {
                        builder.WithOrigins(
                                "http://localhost:5010")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddMvcCore(options =>
                {
                    options.Filters.Add(typeof(ValidateModelAttribute));
                    options.Filters.Add(typeof(StatusCodeFilter));
                })
                .AddApiExplorer()
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssemblyContaining<DTO.DependencyInjectionModule>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(0, 0, "unversioned");
            });
            
            services.AddSwagger();

            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            var apiSettings = _configuration.GetSection("APISettings").Get<ApiSettings>();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
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
                });
            
            services.AddAuthorization();
        }
        
        public void ConfigureBaseServices(IServiceCollection services, ServiceLifetime lifetime)
        {
            services.Configure<ApiSettings>(_configuration.GetSection("APISettings"));
            services.Configure<GoogleSettings>(_configuration.GetSection("GoogleOAuth"));
            services.Configure<RabbitConnectionModel>(_configuration.GetSection("RabbitConnection"));
            services.Configure<FileTemplateUrlModel>(_configuration.GetSection("FileTemplateUrl"));
            services.Configure<FileRootDirectoryModel>(_configuration.GetSection("FileRootDirectory"));
            services.Configure<CropSizesModel>(_configuration.GetSection("CropSizes"));
            
            var dbSettings = new DbSettings
            {
                DB_HOST = _configuration.GetSection("DB_HOST").Value,
                DB_PORT = _configuration.GetSection("DB_PORT").Value,
                DB_USER = _configuration.GetSection("DB_USER").Value,
                DB_NAME = _configuration.GetSection("DB_NAME").Value,
                DB_PASSWORD = _configuration.GetSection("DB_PASSWORD").Value
            };
            
            Console.WriteLine($"Connection string: {dbSettings?.PostgresConnectionString}");

            services.AddSerilog(dbSettings);

            services.AddDbContext<SquadioDbContext>(builder =>
                builder
                    .EnableSensitiveDataLogging()
                    .UseNpgsql(dbSettings.PostgresConnectionString,
                        optionsBuilder =>
                            optionsBuilder.MigrationsAssembly(typeof(SquadioDbContext).Assembly.FullName)));

            DependencyInjectionModule.Load(services, lifetime);

            services.AddSingleton(MappingConfig.GetMapper());

            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app
            , IWebHostEnvironment env
            , IHostApplicationLifetime hostLifetime
            , IApiVersionDescriptionProvider apiVersionProvider
            , ILogger<Startup> logger)
        {
            hostLifetime.SerilogRegisterCloseAndFlush();
            
            logger.LogInformation("Startup.Configure: Start");
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();
            app.UseCors(MyAllowSquadioOrigins);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware(typeof(ExceptionMiddleware));
            app.UseDefaultFiles();
            app.UseStaticFiles();
            
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                for (var i = apiVersionProvider.ApiVersionDescriptions.Count() - 1; i >= 0; i--)
                    options.SwaggerEndpoint($"{apiVersionProvider.ApiVersionDescriptions[i].GroupName}/swagger.json"
                        , apiVersionProvider.ApiVersionDescriptions[i].GroupName.ToUpperInvariant());
                options.DocExpansion(DocExpansion.None);
            });

            app.EnsureMigrationOfContext<SquadioDbContext>();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            logger.LogInformation("Startup.Configure: Finish");
        }
    }
}
