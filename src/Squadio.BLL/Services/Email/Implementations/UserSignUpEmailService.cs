﻿using Microsoft.Extensions.Options;
using Squadio.BLL.Extensions;
using Squadio.BLL.Services.Email.Sender;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Email.Implementations
{
    public class UserSignUpEmailService: BaseEmailService<UserSignUpEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public UserSignUpEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(UserSignUpEmailModel model)
        {
            
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.BLL.Services.Email.Templates.SignUpRequestTemplate.html")
                .Replace("{{SetPasswordUrl}}", _options.Value.SetPasswordUrl)
                .Replace("{{Code}}", model.Code);

            return resource;
        }

        protected override string GetSubject(UserSignUpEmailModel emailModel)
        {
            return "SignUp request";
        }
    }
}