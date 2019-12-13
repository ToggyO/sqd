﻿using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
using Squadio.EmailSender.EmailService.Sender;
using Squadio.EmailSender.Extensions;

namespace Squadio.EmailSender.EmailService.Implementations
{
    public class ResetPasswordAdminEmailService: BaseEmailService<PasswordRestoreAdminEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public ResetPasswordAdminEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(PasswordRestoreAdminEmailModel model)
        {
            
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.EmailSender.EmailService.Templates.ResetPasswordTemplate.html")
                .Replace("{{ResetPasswordUrl}}", _options.Value.ResetPasswordAdminUrl)
                .Replace("{{Code}}", model.Code);

            return resource;
        }

        protected override string GetSubject(PasswordRestoreAdminEmailModel emailModel)
        {
            return "Reset your Squad password";
        }
    }
}