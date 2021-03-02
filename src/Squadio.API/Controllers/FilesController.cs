using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Resources;

namespace Squadio.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/files")]
    public class FilesController: Controller
    {
        private readonly IFilesHandler _handler;

        public FilesController(IFilesHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("{group}/{resolution}/{filename}")]
        public async Task<IActionResult> GetFile([Required] string group
            , [Required] string resolution
            , [Required] string filename)
        {
            return await _handler.GetFile(group, filename, resolution);
        }

        [HttpGet("{group}/{filename}")]
        public async Task<IActionResult> GetFile([Required] string group
            , [Required] string filename)
        {
            return await _handler.GetFile(group, filename);
        }
    }
}
