using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Email.Sender.Implementation
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        private readonly string _smtpServer;
        private readonly string _user;
        private readonly string _password;
        private readonly int _smtpPort;
        private readonly string _from;

        public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSettingsModel> settings)
        {
            _smtpServer = settings.Value.SmtpServer;
            _smtpPort = settings.Value.SmtpPort;
            _from = settings.Value.Name;
            _user = settings.Value.Email;
            _password = settings.Value.Password;
            _logger = logger;
        }


        public void Send([EmailAddress]string to, string subject, string content, EmailAttachment[] attachmentItems = null, string styles = "")
        {
            try
            {
                
                var email = new MailMessage(_from, to)
                {
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = content
                };

                // information
                if (!string.IsNullOrEmpty(styles))
                {
                    email.Body = email.Body.Replace("@@STYLES@@", File.ReadAllText(styles));
                }

                if (attachmentItems != null)
                {
                    var idx = 1;
                    foreach (var attach in attachmentItems)
                    {
                        var filestream = File.OpenRead(attach.FilePath);
                        var inline = new Attachment(filestream, attach.FileName);

                        if (attach.IsInline)
                        {
                            attach.FileName = Path.GetFileNameWithoutExtension(attach.FileName);
                            inline.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        }
                        else
                        {
                            inline.ContentDisposition.DispositionType = DispositionTypeNames.Attachment;
                        }

                        inline.ContentDisposition.Inline = attach.IsInline;

                        inline.ContentId = "attach" + "@" + attach.FileName;
                        inline.ContentType.MediaType = attach.ContentMediaType;
                        inline.ContentType.Name = "attach";
                        email.Attachments.Add(inline);

                        email.Body = email.Body.Replace("@@ATTACH" + idx + "@@", "cid:" + inline.ContentId);

                        idx++;
                    }
                }

                var smtp = new SmtpClient(_smtpServer, _smtpPort)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_user, _password),
                    EnableSsl = true
                };

                _logger.LogInformation(email.Body);

                smtp.Send(email);

                email.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task SendAsync([EmailAddress]string to, string subject, string content, EmailAttachment[] attachmentItems = null, string styles = "")
        {
            try
            {
                /*
                #if Debug
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                #endif
                */

                using (var email = new MailMessage(new MailAddress(_user, _from), new MailAddress(to)))
                {
                    email.Subject = subject;
                    email.IsBodyHtml = true;
                    email.Body = content;

                    // information
                    if (!string.IsNullOrEmpty(styles))
                    {
                        email.Body = email.Body.Replace("@@STYLES@@", File.ReadAllText(styles));
                    }

                    if (attachmentItems != null)
                    {
                        var idx = 1;
                        foreach (var attach in attachmentItems)
                        {
                            var filestream = File.OpenRead(attach.FilePath);
                            var inline = new Attachment(filestream, attach.FileName);

                            if (attach.IsInline)
                            {
                                attach.FileName = Path.GetFileNameWithoutExtension(attach.FileName);
                                inline.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                            }
                            else
                            {
                                inline.ContentDisposition.DispositionType = DispositionTypeNames.Attachment;
                            }

                            inline.ContentDisposition.Inline = attach.IsInline;

                            inline.ContentId = "attach" + "@" + attach.FileName;
                            inline.ContentType.MediaType = attach.ContentMediaType;
                            inline.ContentType.Name = "attach";
                            email.Attachments.Add(inline);

                            email.Body = email.Body.Replace("@@ATTACH" + idx + "@@", "cid:" + inline.ContentId);

                            idx++;
                        }
                    }

                    using (var smtp = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(_user, _password);
                        smtp.EnableSsl = true;
                        
                        //_logger.LogInformation(email.Body);

                        await smtp.SendMailAsync(email);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}