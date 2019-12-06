using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.API.WebSocketHubHandlers.Projects;
using Squadio.API.WebSocketHubs;
using Squadio.Common.WebSocket;

namespace Squadio.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/versions")]
    public class VersionController : ControllerBase
    {
        private const string Version = "0.2.6 b";
        private readonly ILogger<VersionController> _logger;
        private readonly IProjectHubHandler _projectHubHandler;

        public VersionController(ILogger<VersionController> logger
            , IProjectHubHandler projectHubHandler)
        {
            _logger = logger;
            _projectHubHandler = projectHubHandler;
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
        
        [HttpGet("ws/{teamId}")]
        [AllowAnonymous]
        public async Task<string> WS([FromRoute] Guid teamId)
        {
            await _projectHubHandler.BroadcastTeamChanges(new BroadcastTeamChangesModel
            {
                TeamId = teamId.ToString(),
                Token = ""
            });
            return "Done";
        }
    }
}