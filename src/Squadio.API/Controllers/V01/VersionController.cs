using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Squadio.Common.Models.Responses;

namespace Squadio.API.Controllers.V01
{
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("0.1")]
    [Route("api/v{version:apiVersion}/versions")]
    public class VersionController : ControllerBase
    {
        private const string Version = "0.0.0 b";
        private readonly ILogger<VersionController> _logger;

        public VersionController(ILogger<VersionController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get current version of API
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public Response<string> GetVersion()
        {
            return new Response<string>
            {
                Data = Version
            };
        }
    }
}