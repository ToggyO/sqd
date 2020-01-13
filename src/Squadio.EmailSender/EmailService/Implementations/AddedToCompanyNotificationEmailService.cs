using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
using Squadio.EmailSender.EmailService.Sender;
using Squadio.EmailSender.Extensions;

namespace Squadio.EmailSender.EmailService.Implementations
{
    public class AddedToCompanyNotificationEmailService: BaseEmailService<AddToCompanyEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public AddedToCompanyNotificationEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(AddToCompanyEmailModel model)
        {
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.EmailSender.EmailService.Templates.AddedNotificationTemplate.html")
                .Replace("{{AuthorName}}", model.AuthorName)
                .Replace("{{EntityName}}", model.CompanyName)
                .Replace("{{EntityType}}", "company");

            return resource;
        }

        protected override string GetSubject(AddToCompanyEmailModel emailModel)
        {
            return $"{emailModel.AuthorName} added you to {emailModel.CompanyName} company";
        }
    }
}