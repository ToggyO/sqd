using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.API.Filters;

namespace Squadio.API.WebSockerHubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        
        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
            _logger.LogInformation("ChatHub constructor work done");
        }
        
        [HubMethodName("SendMessage")]
        public async Task SendMessage(SimpleMessage model)
        {
            _logger.LogInformation($"Enter into ChatHub.SendMessage from '{model.Username}' with message: {model.Message}");
            try
            {
                await Clients.All.SendAsync("ReceiveMessage" , model);
            }
            catch (Exception e)
            {
                _logger.LogError($"ChatHub.SendMessage throw: {e.Message}");
            }
        }
    }

    public class SimpleMessage
    {
        public string Username { get; set; } = "Anonymous";
        public string Message { get; set; }
    }
}