using System;
using System.Threading.Tasks;
using Squadio.BLL.Providers.WebSocket.BaseHubProvider;
using Squadio.Common.WebSocket;

namespace Squadio.BLL.Providers.WebSocket.Sidebar
{
    public class SidebarHubProvider : ISidebarHubProvider
    {
        private readonly IBaseHubProvider _baseHubProvider;
        
        public SidebarHubProvider(IBaseHubProvider baseHubProvider)
        {
            _baseHubProvider = baseHubProvider;
        }
        public Task BroadcastProjectChanges(Guid groupId, BroadcastSidebarProjectChangesModel model)
        {
            throw new NotImplementedException();
        }

        public Task BroadcastFolderChanges(Guid groupId, BroadcastSidebarFolderChangesModel model)
        {
            throw new NotImplementedException();
        }

        public Task BroadcastBoardChanges(Guid groupId, BroadcastSidebarBoardChangesModel model)
        {
            throw new NotImplementedException();
        }
    }
}