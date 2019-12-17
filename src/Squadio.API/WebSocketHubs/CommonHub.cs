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
    public class CommonHub : Hub
    {
        private readonly ILogger<CommonHub> _logger;
        private readonly IUsersProvider _usersProvider;
        private readonly GroupUsersDictionary<Guid> _dictionary;
        
        
        public CommonHub(ILogger<CommonHub> logger
            , IUsersProvider usersProvider)
        {
            _logger = logger;
            _usersProvider = usersProvider;
            _dictionary = GroupUsersDictionary<Guid>.GetInstance();
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

        [HubMethodName(EndpointsWS.Sidebar.SubscribeTeam)]
        public async Task SidebarSubscribeTeam(SubscribeToSidebarModel model)
        {
            var userResponse = await _usersProvider.GetById(Context.User.GetUserId());
            if (!userResponse.IsSuccess)
            {
                Context.Abort();
                return;
            }

            var user = userResponse.Data;
            
            _dictionary.Add(model.TeamId, ConnectionGroup.Sidebar, user.Id, Context.ConnectionId);
        }
    }
}