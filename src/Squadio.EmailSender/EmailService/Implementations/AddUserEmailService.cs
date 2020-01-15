using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
using Squadio.EmailSender.EmailService.Sender;
using Squadio.EmailSender.Extensions;

namespace Squadio.EmailSender.EmailService.Implementations
{
    public class AddUserEmailService: BaseEmailService<AddUserEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public AddUserEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(AddUserEmailModel model)
        {
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.EmailSender.EmailService.Templates.AddedNotificationTemplate.html")
                .Replace("{{AuthorName}}", model.AuthorName)
                .Replace("{{EntityName}}", "[name]")
                .Replace("{{EntityType}}", "[type]");

            return resource;
        }

        protected override string GetSubject(AddUserEmailModel emailModel)
        {
            return $"{emailModel.AuthorName} added you to [name] [type]";
        }
    }
}