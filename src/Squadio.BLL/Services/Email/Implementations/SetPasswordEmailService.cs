using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Squadio.Common.Extensions.EmbeddedResources;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Email.Implementations
{
    public class SetPasswordEmailService : MailService<PasswordSetEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;

        public SetPasswordEmailService(IOptions<StaticUrlsSettingsModel> options
            , IOptions<EmailSettingsModel> emailSettings) : base(emailSettings)
        {
            _options = options;
        }

        public override Task<MailMessage> Get(PasswordSetEmailModel model)
        {
            var body = (EmbeddedResources.GetSetPasswordTemplate())
                .Replace("{{SetPasswordUrl}}", _options.Value.SetPasswordUrl)
                .Replace("{{Code}}", model.Code);

            return Task.FromResult(new MailMessage
            {
                Subject = "Задание пароля",
                Body = body,
                Html = true,
                ToAddresses = new[]
                {
                    model.Email,
                }
            });
        }
    }
}