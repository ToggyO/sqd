using System;

namespace Squadio.Common.WebSocket
{
    public class SubscribeToSidebarModel : BaseWebSocketModel
    {
        public Guid TeamId { get; set; }
    }
}