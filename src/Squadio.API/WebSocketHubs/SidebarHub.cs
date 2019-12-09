using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.Common.WebSocket;

namespace Squadio.API.WebSocketHubs
{
    [Authorize]
    public class SidebarHub : Hub
    {
        private readonly ILogger<SidebarHub> _logger;
        public SidebarHub(ILogger<SidebarHub> logger)
        {
            _logger = logger;
        }

        [HubMethodName("SubscribeToSidebar")]
        public async Task SubscribeToSidebar(SubscribeToSidebarModel model)
        {
            if (!Guid.TryParse(model.TeamId, out var teamGuid))
            {
                _logger.LogWarning($"Can't parse teamId: {model.TeamId}");
                return;
            }

            if (!Guid.TryParse(model.UserId, out var userGuid))
            {
                _logger.LogWarning($"Can't parse userId: {model.UserId}");
                return;
            }

            var groupName = GetGroupName(teamGuid, userGuid);
            if(groupName == null) 
                return;
            
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        
        [HubMethodName("UnsubscribeFromSidebar")]
        public async Task UnsubscribeFromSidebar(UnsubscribeFromSidebarModel model)
        {
            if (!Guid.TryParse(model.TeamId, out var teamGuid))
            {
                _logger.LogWarning($"Can't parse teamId: {model.TeamId}");
                return;
            }

            if (!Guid.TryParse(model.UserId, out var userGuid))
            {
                _logger.LogWarning($"Can't parse userId: {model.UserId}");
                return;
            }
            
            var groupName = GetGroupName(teamGuid, userGuid);
            if(groupName == null) 
                return;
            
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        private string GetGroupName(Guid teamId, Guid userId)
        {
            if (teamId == Guid.Empty || userId == Guid.Empty || teamId == userId)
            {
                _logger.LogWarning($"Can't create group name: \n teamId = {teamId} \n userId = {userId}");
                return null;
            }

            return teamId.ToString("N") + userId.ToString("N");
        }
    }
}