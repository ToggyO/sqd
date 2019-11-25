using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;

namespace Squadio.EmailSender.RabbitMessageHandler.Implementation
{
    public class RabbitMessageHandler: IRabbitMessageHandler
    {
        private static ILogger<RabbitMessageHandler> _logger;
        private static IOptions<EmailSettingsModel> _emailSettings;

        public RabbitMessageHandler(ILogger<RabbitMessageHandler> logger
            , IOptions<EmailSettingsModel> emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings;
        }
        
        public void HandleEmailMessage(UserConfirmEmailModel message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got code: {0}, for {1}", message.Code, message.To);
            Console.ResetColor();
            _logger.LogInformation("Got code: {Code}, for {To}", message.Code, message.To);
        }
    }
}