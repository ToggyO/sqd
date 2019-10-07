using Microsoft.Extensions.Options;
using Squadio.BLL.Extensions;
using Squadio.BLL.Services.Email.Sender;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Email.Implementations
{
    public class SetPasswordEmailService: BaseEmailService<PasswordSetEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public SetPasswordEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(PasswordSetEmailModel model)
        {
            
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.BLL.Services.Email.Templates.SetPasswordTemplate.html")
                .Replace("{{SetPasswordUrl}}", _options.Value.SetPasswordUrl
                .Replace("{{Code}}", model.Code));

            return resource;
        }

        protected override string GetSubject(PasswordSetEmailModel emailModel)
        {
            return "Set password";
        }
    }
}