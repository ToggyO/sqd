namespace Squadio.Common.Models.WebSocket
{
    public static class EndpointsWS
    {
        public static class Sidebar
        {
            public const string Broadcast = "SidebarBroadcast";
            public const string SubscribeTeam = "SidebarSubscribeTeam";
        }
        
        public static class Chat
        {
            public const string SendMessage = "ChatSendMessage";
            public const string ReceiveMessage = "ChatReceiveMessage";
        }
    }
}