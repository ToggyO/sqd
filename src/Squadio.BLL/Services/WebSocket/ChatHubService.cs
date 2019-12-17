using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.Common.Models.WebSocket;

namespace Squadio.BLL.Services.WebSocket
{
    [Authorize]
    public class ChatHubService : Hub
    {
        private readonly ILogger<ChatHubService> _logger;
        
        public ChatHubService(ILogger<ChatHubService> logger)
        {
            _logger = logger;
            _logger.LogInformation("ChatHubService constructor work done");
        }

        [HubMethodName(EndpointsWS.Chat.SendMessage)]
        public async Task SendMessage(SimpleMessage model)
        {
            _logger.LogInformation($"Enter into ChatHubService.SendMessage from '{model.Username}' with message: {model.Message}");
            
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