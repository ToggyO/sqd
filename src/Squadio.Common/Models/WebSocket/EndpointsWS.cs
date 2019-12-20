namespace Squadio.Common.Models.WebSocket
{
    public static class EndpointsWS
    {
        public static class Sidebar
        {
            public const string Broadcast = "SidebarBroadcast";
            public const string BroadcastProjectChanged = "SidebarBroadcastProjectChanged";
            public const string BroadcastFolderChanged = "SidebarBroadcastFolderChanged";
            public const string BroadcastBoardChanged = "SidebarBroadcastBoardChanged";
            public const string SubscribeTeam = "SidebarSubscribeTeam";
        }
        
        public static class Chat
        {
            public const string SendMessage = "ChatSendMessage";
            public const string ReceiveMessage = "ChatReceiveMessage";
        }
    }
}