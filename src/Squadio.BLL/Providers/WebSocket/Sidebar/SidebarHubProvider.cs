using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.WebSocket.BaseHubProvider;
using Squadio.Common.Enums;
using Squadio.Common.Models.WebSocket;
using Squadio.Common.WebSocket;

namespace Squadio.BLL.Providers.WebSocket.Sidebar
{
    public class SidebarHubProvider : ISidebarHubProvider
    {
        private readonly ILogger<SidebarHubProvider> _logger;
        private readonly IBaseHubProvider _baseHubProvider;
        private const ConnectionGroup _group = ConnectionGroup.Sidebar;
        
        public SidebarHubProvider(IBaseHubProvider baseHubProvider
            , ILogger<SidebarHubProvider> logger)
        {
            _baseHubProvider = baseHubProvider;
            _logger = logger;
        }
        public async Task BroadcastProjectChanges(Guid groupId, BroadcastSidebarProjectChangesModel model)
        {
            await _baseHubProvider.BroadcastChanges(groupId, _group, EndpointsWS.Sidebar.Broadcast, model);
        }

        public async Task BroadcastFolderChanges(Guid groupId, BroadcastSidebarFolderChangesModel model)
        {
            await _baseHubProvider.BroadcastChanges(groupId, _group, EndpointsWS.Sidebar.Broadcast, model);
        }

        public async Task BroadcastBoardChanges(Guid groupId, BroadcastSidebarBoardChangesModel model)
        {
            await _baseHubProvider.BroadcastChanges(groupId, _group, EndpointsWS.Sidebar.Broadcast, model);
        }
    }
}