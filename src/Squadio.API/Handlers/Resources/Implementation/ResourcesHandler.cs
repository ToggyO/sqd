using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Squadio.API.Handlers.Resources.Implementation
{
    public class ResourcesHandler : IResourcesHandler
    {
        public Task<FileContentResult> GetFile(string group, string resolution, string filename)
        {
            throw new System.NotImplementedException();
        }
    }
}