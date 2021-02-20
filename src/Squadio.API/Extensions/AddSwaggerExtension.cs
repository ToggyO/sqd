using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Squadio.API.Extensions
{
    public static class AddSwaggerExtension
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                for (var i = provider.ApiVersionDescriptions.Count() - 1; i >= 0; i--)
                    options.SwaggerDoc(provider.ApiVersionDescriptions[i].GroupName
                        , new OpenApiInfo {Title = "Squad.io API", Version = provider.ApiVersionDescriptions[i].ApiVersion.ToString()});

                options.OperationFilter<SwaggerRemoveVersionParameters>();
                options.DocumentFilter<SwaggerSetVersionInPaths>();

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                options.AddSecurityDefinition("Bearer", securitySchema);

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        securitySchema,
                        new string[0]
                    }
                });


                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            services.AddApiVersioning(opts =>
            {
                opts.ReportApiVersions = true;
                opts.AssumeDefaultVersionWhenUnspecified = true;
                opts.DefaultApiVersion = new ApiVersion(0, 1);
            });
        }
    }
}