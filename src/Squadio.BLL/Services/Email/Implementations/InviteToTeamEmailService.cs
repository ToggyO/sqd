﻿using Microsoft.Extensions.Options;
using Squadio.BLL.Services.Email.Sender;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Email.Implementations
{
    public class InviteToTeamEmailService: BaseEmailService<InviteToTeamEmailModel>
    {
        private readonly IOptions<StaticUrlsSettingsModel> _options;
        
        public InviteToTeamEmailService(IOptions<EmailSettingsModel> emailSettings
            , IEmailSender emailSender
            , IOptions<StaticUrlsSettingsModel> options) : base(emailSettings, emailSender)
        {
            _options = options;
        }

        protected override string GetHtmlTemplate(InviteToTeamEmailModel model)
        {
            var resource = "";

            return resource;
        }

        protected override string GetSubject(InviteToTeamEmailModel emailModel)
        {
            return $"{emailModel.AuthorName} invited you to Squad";
        }
    }
}