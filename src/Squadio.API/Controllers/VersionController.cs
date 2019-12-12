using Mapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/versions")]
    public class VersionController : ControllerBase
    {
        private const string Version = "0.4.2 b";
        private readonly ILogger<VersionController> _logger;
        private readonly IMapper _mapper;

        public VersionController(ILogger<VersionController> logger
            , IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
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