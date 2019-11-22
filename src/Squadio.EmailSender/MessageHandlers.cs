using System;
using Microsoft.Extensions.Logging;
using Squadio.Common.Models.Email;

namespace Squadio.EmailSender
{
    public class MessageHandlers
    {
        private static ILogger<MessageHandlers> _logger;

        public MessageHandlers(ILogger<MessageHandlers> logger)
        {
            _logger = logger;
        }
        
        public static void HandleEmailMessage(UserConfirmEmailModel message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got code: {0}, for {1}", message.Code, message.To);
            Console.ResetColor();
            _logger.LogInformation("Got code: {Code}, for {To}", message.Code, message.To);
        }
    }
}