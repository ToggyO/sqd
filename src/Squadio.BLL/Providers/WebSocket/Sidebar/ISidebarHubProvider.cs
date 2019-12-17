using System;
using System.Threading.Tasks;
using Squadio.Common.WebSocket;

namespace Squadio.BLL.Providers.WebSocket.Sidebar
{
    public interface ISidebarHubProvider
    {
        Task BroadcastProjectChanges(Guid groupId, BroadcastSidebarProjectChangesModel model);
        Task BroadcastFolderChanges(Guid groupId, BroadcastSidebarFolderChangesModel model);
        Task BroadcastBoardChanges(Guid groupId, BroadcastSidebarBoardChangesModel model);
    }
}