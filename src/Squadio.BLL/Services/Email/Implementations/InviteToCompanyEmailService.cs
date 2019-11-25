﻿using Microsoft.Extensions.Options;
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
            var resource = "";

            return resource;
        }

        protected override string GetSubject(InviteToCompanyEmailModel emailModel)
        {
            return $"{emailModel.AuthorName} invited you to {emailModel.CompanyName} company";
        }
    }
}