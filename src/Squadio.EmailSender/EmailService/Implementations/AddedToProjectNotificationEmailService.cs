using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
using Squadio.EmailSender.EmailService.Sender;
using Squadio.EmailSender.Extensions;

namespace Squadio.EmailSender.EmailService.Implementations
{
    public class AddedToProjectNotificationEmailService: BaseEmailService<AddToProjectEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public AddedToProjectNotificationEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(AddToProjectEmailModel model)
        {
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.EmailSender.EmailService.Templates.AddedNotificationTemplate.html")
                .Replace("{{AuthorName}}", model.AuthorName)
                .Replace("{{EntityName}}", model.ProjectName)
                .Replace("{{EntityType}}", "project");

            return resource;
        }

        protected override string GetSubject(AddToProjectEmailModel emailModel)
        {
            return $"{emailModel.AuthorName} added you to {emailModel.ProjectName} project";
        }
    }
}