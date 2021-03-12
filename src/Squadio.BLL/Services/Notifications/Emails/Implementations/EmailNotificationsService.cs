using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.Common.Settings;
using Squadio.DTO.Models.Notifications;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using Squadio.Common.Models.Emails;
using Squadio.Common.Models.Responses;
using Response = Squadio.Common.Models.Responses.Response;

namespace Squadio.BLL.Services.Notifications.Emails.Implementations
{
    public class EmailNotificationsService : IEmailNotificationsService
    {
        private readonly SmtpSettings _smtpCredentials;
        private readonly StaticUrls _urls;
        private readonly FileDirectoryPathSettings _fileDirectoryPath;
        private readonly ILogger<EmailNotificationsService> _logger;

        public EmailNotificationsService(IOptions<SmtpSettings> emailCredentials
            , IOptions<StaticUrls> urls
            , IOptions<FileDirectoryPathSettings> fileDirectoryPath
            , ILogger<EmailNotificationsService> logger)
        {
            _smtpCredentials = emailCredentials.Value;
            _urls = urls.Value;
            _fileDirectoryPath = fileDirectoryPath.Value;
            _logger = logger;
        }

        public async Task<Response> SendEmail(EmailMessageDTO message)
        {
            var templatePath = $"{_fileDirectoryPath.EmailTemplatePath}/{message.TemplateId}.html";
            
            var mailNotificationModel = new MailNotificationModel
            {
                Body = message.Body,
                Html = message.Html,
                Subject = message.Subject,
                Args = message.Args,
                TemplatePath = message.TemplateId != null 
                    ? $"{_fileDirectoryPath.EmailTemplatePath}/{message.TemplateId}.html" 
                    : null,
                ToAddresses = message.ToEmails,
                FromName = message.FromName
            };

            return await SendEmail(mailNotificationModel);
        }

        public async Task<Response> SendResetAdminPasswordEmail(string email, string code)
        {
            var args = new Dictionary<string, string>
            {
                {"{{Code}}", code},
                {"{{ResetAdminPasswordUrl}}",_urls.ResetAdminPasswordUrl}
            };
            var mailNotificationModel = new EmailMessageDTO
            {
                Html = true,
                Subject = "Reset password",
                Args = args,
                TemplateId = TemplateId.ResetAdminPassword,
                ToEmails = new []{email},
            };

            return await SendEmail(mailNotificationModel);
        }

        public async Task<Response> SendConfirmNewAdminMailboxEmail(string newEmail, string code)
        {
            var args = new Dictionary<string, string>
            {
                {"{{Code}}", code},
                {"{{ConfirmAdminNewMailboxUrl}}",_urls.ConfirmAdminNewMailboxUrl}
            };
            var mailNotificationModel = new EmailMessageDTO
            {
                Html = true,
                Subject = "Confirm email",
                Args = args,
                TemplateId = TemplateId.ConfirmAdminNewMailbox,
                ToEmails = new []{newEmail},
            };

            return await SendEmail(mailNotificationModel);
        }

        public async Task<Response> SendResetPasswordEmail(string email, string code)
        {
            var args = new Dictionary<string, string>
            {
                {"{{Code}}", code},
                {"{{ResetPasswordUrl}}",_urls.ResetPasswordUrl}
            };
            var mailNotificationModel = new EmailMessageDTO
            {
                Html = true,
                Subject = "Reset password",
                Args = args,
                TemplateId = TemplateId.ResetPassword,
                ToEmails = new []{email},
            };

            return await SendEmail(mailNotificationModel);
        }

        public async Task<Response> SendConfirmNewMailboxEmail(string newEmail, string code)
        {
            var args = new Dictionary<string, string>
            {
                {"{{Code}}", code},
                {"{{ConfirmNewMailboxUrl}}",_urls.ConfirmNewMailboxUrl}
            };
            var mailNotificationModel = new EmailMessageDTO
            {
                Html = true,
                Subject = "Confirm email",
                Args = args,
                TemplateId = TemplateId.ConfirmNewMailbox,
                ToEmails = new []{newEmail},
            };

            return await SendEmail(mailNotificationModel);
        }

        public async Task<Response> SendInviteEmail(string email, string authorName, string entityName, string entityType, string code)
        {
            var args = new Dictionary<string, string>
            {
                {"{{Code}}", code},
                {"{{InviteUserPageUrl}}", _urls.InviteUserPageUrl},
                {"{{AuthorName}}", authorName},
                {"{{EntityName}}", entityName},
                {"{{EntityType}}", entityType}
            };
            var mailNotificationModel = new EmailMessageDTO
            {
                Html = true,
                Subject = "Invite to squad.io",
                Args = args,
                TemplateId = TemplateId.InviteUser,
                ToEmails = new []{email},
            };

            return await SendEmail(mailNotificationModel);
        }

        // private async Task<Response> SendEmail(MailNotificationModel message)
        // {
        //     //TODO: maybe change implementation of sending email
        //     try
        //     {
        //         message.FromEmail = _smtpCredentials.Email;
        //         message.FromName ??= _smtpCredentials.FromName;
        //         using (var client = new SmtpClient(_smtpCredentials.Server, _smtpCredentials.Port))
        //         {
        //             client.UseDefaultCredentials = false;
        //             client.Credentials = new NetworkCredential(_smtpCredentials.Email, _smtpCredentials.Password);
        //             client.EnableSsl = _smtpCredentials.UseSsl;
        //             using var emailMessage = EmailMessagePrototype.CreateNewEmailFromTemplate(message);
        //             await client.SendMailAsync(emailMessage.GetMessage());
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         _logger.LogError(e, "Can't send emails");
        //         return new ErrorResponse
        //         {
        //             Message = e.Message
        //         };
        //     }
        //     
        //     _logger.LogInformation("Emails send success");
        //     return new Response();
        // }

        private async Task<Response> SendEmail(MailNotificationModel baseMessage)
        {
            try
            {
                var apiKey = _smtpCredentials.SendGridApiKey;
                var client = new SendGridClient(apiKey);

                baseMessage.FromEmail = _smtpCredentials.Email;
                baseMessage.FromName ??= _smtpCredentials.FromName;
                using var emailMessage = EmailMessagePrototype.CreateNewEmailFromTemplate(baseMessage);
                var message = emailMessage.GetMessage();

                var messageSendGrid = new SendGridMessage
                {
                    From = new EmailAddress(_smtpCredentials.Email, _smtpCredentials.FromName),
                    HtmlContent = message.Body,
                    PlainTextContent = message.Body,
                    Subject = message.Subject,
                    TemplateId = null
                };

                foreach (var address in message.To)
                {
                    messageSendGrid.AddTo(address.Address);
                }

                var response = await client.SendEmailAsync(messageSendGrid);
                _logger.LogInformation($"Email sending status: {response.StatusCode}");

                var rawResponse = await response.Body.ReadAsStringAsync();
                _logger.LogInformation($"SendGridResponse: {rawResponse}");

                if(response.StatusCode != HttpStatusCode.Accepted)
                    throw new Exception(rawResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Can't send emails using SendGrid");
                return new ErrorResponse
                {
                    Message = e.Message
                };
            }
            return new Response();
        }
    }
}