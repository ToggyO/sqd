using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Squadio.API.Extensions
{
    public class SwaggerRemoveVersionParameters: IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.SingleOrDefault(p => p.Name == "version");
            if (versionParameter == null) return;

            operation.Parameters.Remove(versionParameter);
        }
    }
}
