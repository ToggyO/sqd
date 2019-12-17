using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.Common.WebSocket;

namespace Squadio.API.WebSocketHubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        
        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
            _logger.LogInformation("ChatHub constructor work done");
        }

        [HubMethodName(EndpointsWS.Chat.SendMessage)]
        public async Task SendMessage(SimpleMessage model)
        {
            _logger.LogInformation($"Enter into ChatHub.SendMessage from '{model.Username}' with message: {model.Message}");
            
            var message = model.Message;
            var newMessage = new SimpleMessage
            {
                Message = message,
                Username = model.Username
            };

            await Clients.All.SendAsync(EndpointsWS.Chat.ReceiveMessage, newMessage);
        }
    }

    public class SimpleMessage
    {
        public string Username { get; set; } = "Anonymous";
        public string Message { get; set; }
    }
}