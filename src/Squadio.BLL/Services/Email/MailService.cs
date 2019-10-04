using System.Linq;
using System.Threading.Tasks;

using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;

using Microsoft.Extensions.Options;
using Squadio.Common.Settings;
using MailMessage = Squadio.Common.Models.Email.MailMessage;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Squadio.BLL.Services.Email
{
    public abstract class MailService<T> : IMailService<T>, IMailProvider<T>
    {
        public IOptions<EmailSettingsModel> Settings { get; }

        protected MailService(IOptions<EmailSettingsModel> settings)
        {
            Settings = settings;
        }


        public abstract Task<MailMessage> Get(T model);

        public async Task Send(T model)
        {
            var message = await Get(model);

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(Settings.Value.Name, Settings.Value.Email));
            mailMessage.To.AddRange(message.ToAddresses.Select(x => new MailboxAddress(x)));
            mailMessage.Subject = message.Subject;
            mailMessage.Body = new TextPart(message.Html ? TextFormat.Html : TextFormat.Plain)
            {
                Text = message.Body
            };
            mailMessage.Prepare(EncodingConstraint.EightBit);

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(Settings.Value.SmtpServer, Settings.Value.SmtpPort, true);

                await client.AuthenticateAsync(Settings.Value.Email, Settings.Value.Password);

                await client.SendAsync(mailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}