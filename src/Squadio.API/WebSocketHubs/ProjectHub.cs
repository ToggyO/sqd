using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Squadio.API.WebSocketHubHandlers.Projects;
using Squadio.Common.WebSocket;

namespace Squadio.API.WebSocketHubs
{
    [Authorize]
    public class ProjectHub : Hub
    {
        private readonly IProjectHubHandler _handler;

        public ProjectHub(IProjectHubHandler handler)
        {
            _handler = handler;
        }

        [HubMethodName("AddToGroup")]
        public async Task SubscribeToTeam(SubscribeToTeamModel model)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, model.TeamId);
        }
        
        [HubMethodName("RemoveFromGroup")]
        public async Task UnsubscribeFromTeam(UnsubscribeFromTeamModel model)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, model.TeamId);
        }
        
        [HubMethodName("SendProjects")]
        public async Task BroadcastTeamChanges(BroadcastTeamChangesModel model)
        {
            await _handler.BroadcastTeamChanges(model);
        }
    }
}