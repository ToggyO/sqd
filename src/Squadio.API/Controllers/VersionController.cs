using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/versions")]
    public class VersionController : ControllerBase
    {
        private const string Version = "0.0.7 b"; 

        [HttpGet]
        [AllowAnonymous]
        public string GetVersion()
        {
            return Version;
        }
    }
}