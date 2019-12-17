using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Services.WebSocket;
using Squadio.Common.Enums;
using Squadio.Common.WebSocket;

namespace Squadio.BLL.Providers.WebSocket.BaseHubProvider
{
    public class BaseHubProvider : IBaseHubProvider
    {
        private readonly ILogger<BaseHubProvider> _logger;
        private readonly IHubContext<CommonHubService> _hub;
        private readonly GroupUsersDictionary<Guid> _dictionary;

        public BaseHubProvider(ILogger<BaseHubProvider> logger
            , IHubContext<CommonHubService> hub)
        {
            _logger = logger;
            _hub = hub;
            _dictionary = GroupUsersDictionary<Guid>.GetInstance();
        }
        
        public async Task BroadcastSidebarChanges(Guid groupId, ConnectionGroup connectionGroup, string methodName, object model)
        {
            var connections = GetConnections(groupId, connectionGroup);
            await _hub.Clients.Clients(connections).SendAsync(methodName, model);
        }

        private List<string> GetConnections(Guid groupId, ConnectionGroup connectionGroup)
        {
            var connections = new List<string>();
            
            if (groupId != Guid.Empty)
            {
                var userIds = _dictionary.GetUsers(groupId, connectionGroup);

                foreach (var userId in userIds)
                {
                    connections.AddRange(_dictionary.GetConnections(groupId, connectionGroup, userId));
                }
            }

            return connections;
        }
    }
}