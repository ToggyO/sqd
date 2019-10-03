using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Users;
using System.Threading.Tasks;

namespace Squadio.API.Controllers
{
    [ApiController]
    [Route("api/versions")]
    public class VersionController : ControllerBase
    {
        const string version = "0.0.1 b";

        [HttpGet]
        public async Task<string> GetVersion()
        {
            return version;
        }
    }
}