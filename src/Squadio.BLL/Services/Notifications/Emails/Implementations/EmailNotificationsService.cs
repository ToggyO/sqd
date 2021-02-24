using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.Common.Models.Responses;
using Squadio.Common.Settings;
using Squadio.DTO.Models.Notifications;
using System.Net.Mail;
using Squadio.Common.Models.Emails;

namespace Squadio.BLL.Services.Notifications.Emails.Implementations
{
    public class EmailNotificationsService : IEmailNotificationsService
    {
        private readonly SmtpSettings _smtpCredentials;
        private readonly ILogger<EmailNotificationsService> _logger;

        public EmailNotificationsService(IOptions<SmtpSettings> emailCredentials
            , ILogger<EmailNotificationsService> logger)
        {
            _smtpCredentials = emailCredentials.Value;
            _logger = logger;
        }

        public async Task<Response> SendEmail(MailNotificationModel message)
        {
            if (message != null)
            {
                message.FromEmail = _smtpCredentials.Email;
                message.FromName = _smtpCredentials.FromName;
            }
            try
            {
                using (var client = new SmtpClient(_smtpCredentials.Server, _smtpCredentials.Port)
                {
                    UseDefaultCredentials = false,
                    EnableSsl = _smtpCredentials.UseSsl,
                    Credentials = new NetworkCredential(_smtpCredentials.Email, _smtpCredentials.Password)
                })
                {
                    using var emailMessage = EmailMessagePrototype.CreateNewEmailFromTemplate(message);
                    await client.SendMailAsync(emailMessage.GetMessage());
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Can't send email");
                return new ErrorResponse
                {
                    Message = e.Message
                };
            }
            
            return new Response();
        }
    }
}