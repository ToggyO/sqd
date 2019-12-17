using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Mapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Squadio.API.WebSocketHubHandlers.Projects;
using Squadio.Common.Enums;
using Squadio.Common.WebSocket;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/versions")]
    public class VersionController : ControllerBase
    {
        private const string Version = "0.5.2 b";
        private readonly ILogger<VersionController> _logger;
        private readonly IMapper _mapper;
        private readonly ISidebarHubHandler _handler;

        public VersionController(ILogger<VersionController> logger
            , IMapper mapper
            , ISidebarHubHandler handler)
        {
            _logger = logger;
            _mapper = mapper;
            _handler = handler;
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

        /// <summary>
        /// Get current version of API
        /// </summary>
        [HttpGet("ws/{teamId}")]
        [AllowAnonymous]
        public async Task LOL([FromRoute, Required] Guid teamId)
        {
            await _handler.BroadcastSidebarChanges(new BroadcastChangesModel
            {
                EntityId = teamId,
                ConnectionGroup = ConnectionGroup.Sidebar,
                EntityType = EntityType.Team
            });
        }
    }
}