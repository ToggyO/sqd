using Microsoft.Extensions.Options;
using Squadio.BLL.Extensions;
using Squadio.BLL.Services.Email.Sender;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Email.Implementations
{
    public class InviteToCompanyEmailService: BaseEmailService<InviteToCompanyEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public InviteToCompanyEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(InviteToCompanyEmailModel model)
        {
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.BLL.Services.Email.Templates.InviteToCompanyTemplate.html")
                .Replace("{{InviteToProjectPageUrl}}", _options.Value.InviteToCompanyPageUrl)
                .Replace("{{AuthorName}}", model.AuthorName)
                .Replace("{{CompanyName}}", model.CompanyName)
                .Replace("{{Code}}", model.Code);

            return resource;
        }

        protected override string GetSubject(InviteToCompanyEmailModel emailModel)
        {
            return "Invite to Squad.io company";
        }
    }
}