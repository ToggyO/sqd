using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.Users;
using Squadio.Common.Enums;
using Squadio.Common.Extensions;
using Squadio.Common.Models.WebSocket;
using Squadio.Common.WebSocket;

namespace Squadio.BLL.Services.WebSocket
{
    [Authorize]
    public class CommonHubService : Hub
    {
        private readonly ILogger<CommonHubService> _logger;
        private readonly IUsersProvider _usersProvider;
        private readonly GroupUsersDictionary<Guid> _dictionary;
        
        
        public CommonHubService(ILogger<CommonHubService> logger
            , IUsersProvider usersProvider)
        {
            _logger = logger;
            _usersProvider = usersProvider;
            _dictionary = GroupUsersDictionary<Guid>.GetInstance();
            _logger.LogInformation("CommonHubService constructor done");
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("CommonHubService OnConnectedAsync enter");
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
            _logger.LogInformation("CommonHubService OnDisconnectedAsync enter");
            _dictionary.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        [HubMethodName(EndpointsWS.Sidebar.SubscribeTeam)]
        public async Task SidebarSubscribeTeam(SubscribeToSidebarModel model)
        {
            _logger.LogInformation("CommonHubService SidebarSubscribeTeam enter");
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