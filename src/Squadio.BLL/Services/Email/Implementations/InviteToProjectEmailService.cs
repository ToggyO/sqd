﻿using Microsoft.Extensions.Options;
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
            var resource = "";

            return resource;
        }

        protected override string GetSubject(InviteToProjectEmailModel emailModel)
        {
            return $"{emailModel.AuthorName} invited you to {emailModel.ProjectName} project";
        }
    }
}