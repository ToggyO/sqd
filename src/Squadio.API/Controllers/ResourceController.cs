using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Resources;

namespace Squadio.API.Controllers
{
    [Route("api/resources")]
    public class ResourceController: Controller
    {
        private readonly IResourcesHandler _handler;

        public ResourceController(IResourcesHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("{group}/{resolution}/{filename}")]
        public async Task<IActionResult> GetFile([Required] string group
            , [Required] string resolution
            , [Required] string filename)
        {
            return await _handler.GetFile(group, resolution, filename);
        }
    }
}
