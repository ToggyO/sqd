using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.Common.Settings;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/versions")]
    public class VersionController : ControllerBase
    {
        private const string Version = "0.6.1 b";
        private readonly ILogger<VersionController> _logger;
        private readonly IOptions<CropSizesModel> _options;

        public VersionController(ILogger<VersionController> logger
            , IOptions<CropSizesModel> options)
        {
            _logger = logger;
            _options = options;
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
        
        [HttpGet("test")]
        [AllowAnonymous]
        public string Test()
        {
            if (_options == null)
            {
                return "Options is null";
            }
            if (_options.Value == null)
            {
                return "Options.Value is null";
            }
            if (_options.Value.SizesStr == null)
            {
                return "Options.Value.SizesStr is null";
            }

            return $"Res: {_options.Value.SizesStr}";
        }
    }
}
