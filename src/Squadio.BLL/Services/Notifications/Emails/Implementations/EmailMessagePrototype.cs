using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Emails;

namespace Squadio.BLL.Services.Notifications.Emails.Implementations
{
    public class EmailMessagePrototype : IDisposable
    {
        private MailMessage _mailMessage;

        private EmailMessagePrototype()
        {
        }

        public static EmailMessagePrototype CreateNewEmailFromTemplate(MailNotificationModel notificationModel)
        {
            return CreateNewEmail()
                .SetHeader(notificationModel.Meta)
                .SetSubject(notificationModel.Subject)
                .SetReceivers(notificationModel.ToAddresses)
                .SetBccAddresses(notificationModel.BccAddresses)
                .SetCcAddresses(notificationModel.CcAddresses)
                .SetFromName(notificationModel.FromEmail, notificationModel.FromName)
                .SetBody(notificationModel.Body, notificationModel.Html, notificationModel.Args, notificationModel.TemplateId)
                .SetFileAttachment(notificationModel.Attachments);
        }

        public static EmailMessagePrototype CreateNewEmail()
        {
            return new EmailMessagePrototype {_mailMessage = new MailMessage()};
        }

        public EmailMessagePrototype SetFileAttachment(MailMessageAttachment[] files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    _mailMessage.Attachments.Add(new Attachment(new MemoryStream(GetContent(file)), file.Name));
                }
            }

            return this;
        }

        public EmailMessagePrototype SetSubject(string subject)
        {
            _mailMessage.Subject = subject;

            return this;
        }

        public EmailMessagePrototype SetHeader(IDictionary<string, string> meta = null)
        {
            if (meta != null)
            {
                foreach (var (key, value) in meta)
                {
                    _mailMessage.Headers.Add(key, value);
                }
            }

            return this;
        }

        public EmailMessagePrototype SetBody(string body, bool html = false, IDictionary<string, string> args = null, string templateId = null)
        {
            _mailMessage.IsBodyHtml = html;
            _mailMessage.Body = templateId != null 
                ? File.ReadAllText($"../Squadio.BLL/Services/Notifications/Emails/EmailTemplates/{templateId}.html", Encoding.UTF8) 
                : body;

            if(args != null)
                _mailMessage.Body = _mailMessage.Body.Replace(args);

            return this;
        }

        public EmailMessagePrototype SetFromName(string email, string name)
        {
            _mailMessage.From = new MailAddress(email, name);

            return this;
        }

        public MailMessage GetMessage()
        {
            return _mailMessage;
        }

        public void Dispose()
        {
            _mailMessage?.Dispose();
        }

        private EmailMessagePrototype SetReceivers(string[] receivers)
        {
            foreach (var receiver in receivers)
                _mailMessage.To.Add(receiver);

            return this;
        }

        private EmailMessagePrototype SetBccAddresses(string[] receivers)
        {
            if (receivers != null)
            {
                foreach (var receiver in receivers)
                    _mailMessage.Bcc.Add(receiver);
            }

            return this;
        }

        private EmailMessagePrototype SetCcAddresses(string[] receivers)
        {
            if (receivers != null)
            {
                foreach (var receiver in receivers)
                    _mailMessage.CC.Add(receiver);
            }

            return this;
        }

        private byte[] GetContent(MailMessageAttachment model)
        {
            if (model.Content != null && model.Content?.Length > 0)
            {
                return model.Content;
            }

            if (!string.IsNullOrEmpty(model.Url))
            {
                using var client = new WebClient();
                return client.DownloadData(model.Url);
            }

            return null;
        }
    }
}