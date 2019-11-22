using Microsoft.Extensions.Options;
using Squadio.BLL.Extensions;
using Squadio.BLL.Services.Email.Sender;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Email.Implementations
{
    public class InviteToProjectEmailService: BaseEmailService<InviteToProjectEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public InviteToProjectEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(InviteToProjectEmailModel model)
        {
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.BLL.Services.Email.Templates.InviteToProjectTemplate.html")
                .Replace("{{InviteToProjectPageUrl}}", _options.Value.InviteToProjectPageUrl)
                .Replace("{{AuthorName}}", model.AuthorName)
                .Replace("{{ProjectName}}", model.ProjectName)
                .Replace("{{Code}}", model.Code)
                .Replace("{{IsAlreadyRegistered}}", model.IsAlreadyRegistered.ToString().ToLower());

            return resource;
        }

        protected override string GetSubject(InviteToProjectEmailModel emailModel)
        {
            return $"{emailModel.AuthorName} invited you to {emailModel.ProjectName} project";
        }
    }
}