using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Squadio.API.Extensions
{
    public class SwaggerSetVersionInPaths : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var dictionary =
                swaggerDoc.Paths.ToDictionary(path => path.Key.Replace("{version}", swaggerDoc.Info.Version),
                    path => path.Value);
            var openApiPaths = new OpenApiPaths();
            foreach (var (key, value) in dictionary)
            {
                openApiPaths.Add(key, value);
            }
            swaggerDoc.Paths = openApiPaths;
        }
    }
}
