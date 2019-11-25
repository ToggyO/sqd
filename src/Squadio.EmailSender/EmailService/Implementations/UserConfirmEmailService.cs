using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
using Squadio.EmailSender.EmailService.Sender;
using Squadio.EmailSender.Extensions;

namespace Squadio.EmailSender.EmailService.Implementations
{
    public class UserConfirmEmailService: BaseEmailService<UserConfirmEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public UserConfirmEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(UserConfirmEmailModel model)
        {
            
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.EmailSender.EmailService.Templates.ConfirmEmailRequestTemplate.html")
                .Replace("{{Code}}", model.Code);

            return resource;
        }

        protected override string GetSubject(UserConfirmEmailModel emailModel)
        {
            return "Squad verification";
        }
    }
}