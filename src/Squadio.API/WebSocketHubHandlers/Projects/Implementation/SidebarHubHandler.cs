using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Squadio.API.WebSocketHubs;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Providers.Teams;
using Squadio.Common.Enums;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.WebSocket;

namespace Squadio.API.WebSocketHubHandlers.Projects.Implementation
{
    public class SidebarHubHandler : ISidebarHubHandler
    {
        private readonly ILogger<SidebarHubHandler> _logger;
        private readonly IHubContext<CommonHub> _hub;
        private readonly GroupUsersDictionary<Guid> _dictionary;
        private const ConnectionGroup _group = ConnectionGroup.Sidebar;

        public SidebarHubHandler(ILogger<SidebarHubHandler> logger
            , IHubContext<CommonHub> hub)
        {
            _logger = logger;
            _hub = hub;
            _dictionary = GroupUsersDictionary<Guid>.GetInstance();
        }

        public async Task BroadcastSidebarChanges(BroadcastChangesModel model)
        {
            try
            {
                if (model != null && model?.EntityId != Guid.Empty)
                {
                    var userIds = _dictionary.GetUsers(model.EntityId, _group);
                    
                    foreach (var userId in userIds)
                    {
                        var connections = _dictionary.GetConnections(model.EntityId, _group, userId);
                        await _hub.Clients.Clients(connections).SendAsync(EndpointsWS.Sidebar.Broadcast);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }
    }
}