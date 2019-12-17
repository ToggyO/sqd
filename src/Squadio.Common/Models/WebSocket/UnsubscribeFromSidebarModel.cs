using System;

namespace Squadio.Common.WebSocket
{
    public class UnsubscribeFromSidebarModel : BaseWebSocketModel
    {
        public Guid TeamId { get; set; }
    }
}