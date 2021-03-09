using System;
using System.Collections.Generic;
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
        private readonly StaticUrls _urls;
        private readonly ILogger<EmailNotificationsService> _logger;

        public EmailNotificationsService(IOptions<SmtpSettings> emailCredentials
            , IOptions<StaticUrls> urls
            , ILogger<EmailNotificationsService> logger)
        {
            _smtpCredentials = emailCredentials.Value;
            _urls = urls.Value;
            _logger = logger;
        }

        public async Task<Response> SendEmail(MailNotificationModel message)
        {
            //TODO: maybe change implementation of sending email
            try
            {
                message.FromEmail = _smtpCredentials.Email;
                message.FromName ??= _smtpCredentials.FromName;
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
                _logger.LogError(e, "Can't send emails");
                return new ErrorResponse
                {
                    Message = e.Message
                };
            }
            
            _logger.LogInformation("Emails send success");
            return new Response();
        }

        public async Task<Response> SendEmail(EmailMessageDTO message)
        {
            var mailNotificationModel = new MailNotificationModel
            {
                Body = message.Body,
                Html = message.Html,
                Subject = message.Subject,
                Args = message.Args,
                TemplateId = message.TemplateId,
                ToAddresses = message.ToEmails,
                FromName = message.FromName
            };

            return await SendEmail(mailNotificationModel);
        }

        public async Task<Response> SendResetPasswordEmail(string email, string code)
        {
            var args = new Dictionary<string, string>
            {
                {"{{Code}}", code},
                {"{{ResetAdminPasswordUrl}}",_urls.ResetAdminPasswordUrl}
            };
            var mailNotificationModel = new MailNotificationModel
            {
                Html = true,
                Subject = "Reset password",
                Args = args,
                TemplateId = TemplateId.ResetPassword,
                ToAddresses = new []{email},
            };

            return await SendEmail(mailNotificationModel);
        }

        public async Task<Response> SendConfirmNewMailboxEmail(string email, string code)
        {
            var args = new Dictionary<string, string>
            {
                {"{{Code}}", code},
                {"{{ConfirmAdminNewMailboxUrl}}",_urls.ConfirmAdminNewMailboxUrl}
            };
            var mailNotificationModel = new MailNotificationModel
            {
                Html = true,
                Subject = "Confirm email",
                Args = args,
                TemplateId = TemplateId.ConfirmAdminNewMailbox,
                ToAddresses = new []{email},
            };

            return await SendEmail(mailNotificationModel);
        }
    }
}