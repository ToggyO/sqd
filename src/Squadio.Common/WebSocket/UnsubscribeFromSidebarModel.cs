namespace Squadio.Common.WebSocket
{
    public class UnsubscribeFromSidebarModel : BaseWebSocketModel
    {
        public string TeamId { get; set; }
        public string UserId { get; set; }
    }
}