using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Squadio.API.WebSocketHubHandlers.Projects;
using Squadio.Common.WebSocket;

namespace Squadio.API.WebSocketHubs
{
    [Authorize]
    public class SidebarHub : Hub
    {
        private readonly ISidebarHubHandler _handler;

        public SidebarHub(ISidebarHubHandler handler)
        {
            _handler = handler;
        }

        [HubMethodName("SubscribeToSidebar")]
        public async Task SubscribeToSidebar(SubscribeToSidebarModel model)
        {
            if (!Guid.TryParse(model.TeamId, out var teamGuid))
                return;
            if (!Guid.TryParse(model.UserId, out var userGuid))
                return;
            
            var groupName = GetGroupName(teamGuid, userGuid);
            if(groupName == null) 
                return;
            
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        
        [HubMethodName("UnsubscribeFromSidebar")]
        public async Task UnsubscribeFromSidebar(UnsubscribeFromSidebarModel model)
        {
            if (!Guid.TryParse(model.TeamId, out var teamGuid))
                return;
            if (!Guid.TryParse(model.UserId, out var userGuid))
                return;
            
            var groupName = GetGroupName(teamGuid, userGuid);
            if(groupName == null) 
                return;
            
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
        
        [HubMethodName("BroadcastSidebarChanges")]
        public async Task BroadcastTeamChanges(BroadcastSidebarChangesModel model)
        {
            await _handler.BroadcastSidebarChanges(model);
        }

        private string GetGroupName(Guid teamId, Guid userId)
        {
            if (teamId == Guid.Empty || userId == Guid.Empty || teamId == userId)
            {
                return null;
            }

            return teamId.ToString("N") + userId.ToString("N");
        }
    }
}