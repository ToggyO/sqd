using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.Users;
using Squadio.Common.Enums;
using Squadio.Common.Extensions;
using Squadio.Common.WebSocket;

namespace Squadio.API.WebSocketHubs
{
    [Authorize]
    public class SidebarHub : Hub
    {
        private readonly ILogger<SidebarHub> _logger;
        private readonly GroupUsersDictionary<Guid> _groupUsers = GroupUsersDictionary<Guid>.GetInstance();
        private readonly IUsersProvider _usersProvider;
        
        public SidebarHub(ILogger<SidebarHub> logger
            , IUsersProvider usersProvider)
        {
            _logger = logger;
            _usersProvider = usersProvider;
        }

        [HubMethodName("SubscribeToSidebar")]
        public async Task SubscribeToSidebar(SubscribeToSidebarModel model)
        {
            var userResponse = await _usersProvider.GetById(Context.User.GetUserId());
            if(!userResponse.IsSuccess)
                return;
            
            var user = userResponse.Data;
            
            if (!Guid.TryParse(model.TeamId, out var teamGuid))
            {
                _logger.LogWarning($"Can't parse teamId: {model.TeamId}");
                return;
            }
            
            _groupUsers.Add(teamGuid, user.Id);
        }

        public override async Task OnConnectedAsync()
        {
            var userResponse = await _usersProvider.GetById(Context.User.GetUserId());
            if(!userResponse.IsSuccess)
                return;
            
            var user = userResponse.Data;
            
            await Groups.AddToGroupAsync(Context.ConnectionId, user.Id.ToString());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}