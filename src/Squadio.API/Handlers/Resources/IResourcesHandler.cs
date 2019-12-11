using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Squadio.API.Handlers.Resources
{
    public interface IResourcesHandler
    {
        Task<FileContentResult> GetFile(string group, string resolution, string filename);
    }
}