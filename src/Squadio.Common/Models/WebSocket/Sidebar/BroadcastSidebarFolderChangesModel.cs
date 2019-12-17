using System;

namespace Squadio.Common.WebSocket
{
    public class BroadcastSidebarFolderChangesModel : BroadcastSidebarProjectChangesModel
    {
        public Guid FolderId { get; set; }
    }
}