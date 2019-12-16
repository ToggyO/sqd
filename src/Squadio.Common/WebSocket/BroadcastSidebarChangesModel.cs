using System;

namespace Squadio.Common.WebSocket
{
    public class BroadcastSidebarChangesModel : BaseWebSocketModel
    {
        public Guid TeamId { get; set; }
    }
}