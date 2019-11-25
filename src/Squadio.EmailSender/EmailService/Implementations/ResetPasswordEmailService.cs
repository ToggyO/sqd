using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
using Squadio.EmailSender.EmailService.Sender;
using Squadio.EmailSender.Extensions;

namespace Squadio.EmailSender.EmailService.Implementations
{
    public class ResetPasswordEmailService: BaseEmailService<PasswordRestoreEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public ResetPasswordEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(PasswordRestoreEmailModel model)
        {
            
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.EmailSender.EmailService.Templates.ResetPasswordTemplate.html")
                .Replace("{{ResetPasswordUrl}}", _options.Value.ResetPasswordUrl)
                .Replace("{{Code}}", model.Code);

            return resource;
        }

        protected override string GetSubject(PasswordRestoreEmailModel emailModel)
        {
            return "Reset your Squad password";
        }
    }
}