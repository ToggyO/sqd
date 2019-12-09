namespace Squadio.Common.WebSocket
{
    public class SubscribeToSidebarModel : BaseWebSocketModel
    {
        public string TeamId { get; set; }
        public string UserId { get; set; }
    }
}