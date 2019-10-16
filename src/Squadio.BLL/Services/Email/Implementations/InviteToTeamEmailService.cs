﻿using Microsoft.Extensions.Options;
using Squadio.BLL.Extensions;
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
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.BLL.Services.Email.Templates.InviteToTeamTemplate.html")
                .Replace("{{InviteToTeamPageUrl}}", _options.Value.InviteToTeamPageUrl)
                .Replace("{{AuthorName}}", model.AuthorName)
                .Replace("{{TeamName}}", model.TeamName)
                .Replace("{{TeamId}}", model.TeamId)
                .Replace("{{Code}}", model.Code);

            return resource;
        }

        protected override string GetSubject(InviteToTeamEmailModel emailModel)
        {
            return "Invite to Squad.io team";
        }
    }
}