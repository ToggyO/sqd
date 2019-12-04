using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Squadio.API.Filters;
using Squadio.API.Handlers.Users;

namespace Squadio.API.WebSocketHubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IUsersHandler _handler;
        
        public ChatHub(ILogger<ChatHub> logger
            , IUsersHandler handler)
        {
            _logger = logger;
            _handler = handler;
            _logger.LogInformation("ChatHub constructor work done");
        }
        
        [HubMethodName("SendMessage")]
        public async Task SendMessage(SimpleMessage model)
        {
            _logger.LogInformation($"Enter into ChatHub.SendMessage from '{model.Username}' with message: {model.Message}");
            try
            {
                if(model.Message == "throw")
                    throw new Exception();
                var message = model.Message;
                var userResponse = await _handler.GetCurrentUser(Context.User);
                if (userResponse.IsSuccess)
                {
                    if (userResponse.IsSuccess)
                    {
                        message += " ||| " + JsonSerializer.Serialize(userResponse.Data);
                    }
                }
                var newMessage = new SimpleMessage
                {
                    Message = message,
                    Username = model.Username
                };
                
                await Clients.All.SendAsync("ReceiveMessage" , newMessage);
            }
            catch (Exception e)
            {
                _logger.LogError($"ChatHub.SendMessage throw: {e.Message}");
                await Clients.Caller.SendAsync("disconnect");
            }
        }
    }

    public class SimpleMessage
    {
        public string Username { get; set; } = "Anonymous";
        public string Message { get; set; }
    }
}