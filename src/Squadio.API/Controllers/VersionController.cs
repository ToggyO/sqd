using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Mapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.WebSocket.BaseHubProvider;
using Squadio.Common.Enums;
using Squadio.Common.Models.WebSocket;
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
        private readonly IBaseHubProvider _provider;

        public VersionController(ILogger<VersionController> logger
            , IMapper mapper
            , IBaseHubProvider provider)
        {
            _logger = logger;
            _mapper = mapper;
            _provider = provider;
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
            await _provider.BroadcastChanges(
                teamId, 
                ConnectionGroup.Sidebar, 
                EndpointsWS.Sidebar.Broadcast,
                new BroadcastSidebarProjectChangesModel
                {
                    TeamId = teamId,
                    ProjectId = Guid.NewGuid()
                });
        }
    }
}