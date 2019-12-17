using System;

namespace Squadio.Common.WebSocket
{
    public class BroadcastSidebarBoardChangesModel : BroadcastSidebarFolderChangesModel
    {
        public Guid BoardId { get; set; }
    }
}