using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
using Squadio.EmailSender.EmailService.Sender;
using Squadio.EmailSender.Extensions;

namespace Squadio.EmailSender.EmailService.Implementations
{
    public class InviteUserEmailService: BaseEmailService<InviteUserEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public InviteUserEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(InviteUserEmailModel model)
        {
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.EmailSender.EmailService.Templates.InviteToTeamTemplate.html")
                .Replace("{{InviteToTeamPageUrl}}", _options.Value.InviteToTeamPageUrl)
                .Replace("{{AuthorName}}", model.AuthorName)
                .Replace("{{EntityName}}", "[name]")
                .Replace("{{EntityType}}", "[type]")
                .Replace("{{Code}}", model.Code)
                .Replace("{{IsAlreadyRegistered}}", model.IsAlreadyRegistered.ToString().ToLower());

            return resource;
        }

        protected override string GetSubject(InviteUserEmailModel emailModel)
        {
            return $"{emailModel.AuthorName} invited you to Squad";
        }
    }
}