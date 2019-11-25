﻿using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
using Squadio.EmailSender.EmailService.Sender;
using Squadio.EmailSender.Extensions;

namespace Squadio.EmailSender.EmailService.Implementations
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
            var resource = EmbeddedResources
                .GetResource(
                    "Squadio.EmailSender.EmailService.Templates.InviteToCompanyTemplate.html")
                .Replace("{{InviteToProjectPageUrl}}", _options.Value.InviteToCompanyPageUrl)
                .Replace("{{AuthorName}}", model.AuthorName)
                .Replace("{{CompanyName}}", model.CompanyName)
                .Replace("{{Code}}", model.Code)
                .Replace("{{IsAlreadyRegistered}}", model.IsAlreadyRegistered.ToString().ToLower());

            return resource;
        }

        protected override string GetSubject(InviteToCompanyEmailModel emailModel)
        {
            return $"{emailModel.AuthorName} invited you to {emailModel.CompanyName} company";
        }
    }
}