using System;
using System.Threading.Tasks;
using Google.Apis.Util;
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
        private readonly IUsersProvider _usersProvider;
        private readonly GroupUsersDictionary<Guid> _dictionary;
        
        public SidebarHub(ILogger<SidebarHub> logger
            , IUsersProvider usersProvider
            , GroupUsersDictionary<Guid> dictionary)
        {
            _logger = logger;
            _usersProvider = usersProvider;
            _dictionary = dictionary;
        }

        public override async Task OnConnectedAsync()
        {
            var userResponse = await _usersProvider.GetById(Context.User.GetUserId());
            if (!userResponse.IsSuccess)
            {
                Context.Abort();
                return;
            }

            var user = userResponse.Data;
            
            await Groups.AddToGroupAsync(Context.ConnectionId, user.Id.ToString());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _dictionary.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("SubscribeTeam")]
        public async Task SubscribeTeam(SubscribeToSidebarModel model)
        {
            var userResponse = await _usersProvider.GetById(Context.User.GetUserId());
            if (!userResponse.IsSuccess)
            {
                Context.Abort();
                return;
            }

            var user = userResponse.Data;
            
            _logger.LogInformation($"User {user.Name} subscribed to team with ID {model.TeamId}");
            
            _dictionary.Add(model.TeamId, user.Id, Context.ConnectionId);
            
            var group = Clients.Groups(user.Id.ToString());
            await group.SendAsync("BroadcastProjects");
        }
    }
}