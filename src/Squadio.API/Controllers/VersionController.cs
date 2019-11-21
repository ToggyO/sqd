using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/versions")]
    public class VersionController : ControllerBase
    {
        private const string Version = "0.1.6 b";

        public VersionController()
        {
        }

        /// <summary>
        /// Get current version of API
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public string GetVersion()
        {
            return Version;
        }
    }
}