using Microsoft.Extensions.Options;
using Squadio.BLL.Extensions;
using Squadio.BLL.Services.Email.Sender;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Email.Implementations
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
                    "Squadio.BLL.Services.Email.Templates.ConfirmEmailRequestTemplate.html")
                .Replace("{{Code}}", model.Code);

            return resource;
        }

        protected override string GetSubject(UserConfirmEmailModel emailModel)
        {
            return "SignUp request";
        }
    }
}