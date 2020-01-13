using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
using Squadio.EmailSender.EmailService.Sender;
using Squadio.EmailSender.Extensions;

namespace Squadio.EmailSender.EmailService.Implementations
{
    public class AddedToTeamNotificationEmailService: BaseEmailService<AddToTeamEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public AddedToTeamNotificationEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(AddToTeamEmailModel model)
        {
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.EmailSender.EmailService.Templates.AddedNotificationTemplate.html")
                .Replace("{{AuthorName}}", model.AuthorName)
                .Replace("{{EntityName}}", model.TeamName)
                .Replace("{{EntityType}}", "team");

            return resource;
        }

        protected override string GetSubject(AddToTeamEmailModel emailModel)
        {
            return $"{emailModel.AuthorName} added you to {emailModel.TeamName} team";
        }
    }
}