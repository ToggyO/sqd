using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Squadio.Common.Extensions.EmbeddedResources;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Email.Implementations
{
    public class ResetPasswordEmailService: MailService<ResetPasswordEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;

        public ResetPasswordEmailService(IOptions<StaticUrlsSettingsModel> options
            , IOptions<EmailSettingsModel> emailSettings) : base(emailSettings)
        {
            _options = options;
        }

        public override Task<MailMessage> Get(ResetPasswordEmailModel model)
        {
            var body = (EmbeddedResources.GetRestorePasswordTemplate())
                .Replace("{{RestoreUrl}}", _options.Value.ResetPasswordUrl)
                .Replace("{{Code}}", model.Code);

            return Task.FromResult(new MailMessage
            {
                Subject = "Восстановление пароля",
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
