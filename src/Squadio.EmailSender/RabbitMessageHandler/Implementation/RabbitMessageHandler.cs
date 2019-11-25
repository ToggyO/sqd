using System;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
using Squadio.EmailSender.EmailService;

namespace Squadio.EmailSender.RabbitMessageHandler.Implementation
{
    public class RabbitMessageHandler: IRabbitMessageHandler
    {
        private readonly ILogger<RabbitMessageHandler> _logger;
        private readonly IEmailService<UserConfirmEmailModel> _userConfirmMailService;
        private readonly IEmailService<PasswordRestoreEmailModel> _passwordRestoreMailService;
        private readonly IEmailService<InviteToCompanyEmailModel> _inviteToCompanyMailService;
        private readonly IEmailService<InviteToTeamEmailModel> _inviteToTeamMailService;
        private readonly IEmailService<InviteToProjectEmailModel> _inviteToProjectMailService;

        public RabbitMessageHandler(ILogger<RabbitMessageHandler> logger
            , IEmailService<UserConfirmEmailModel> userConfirmMailService
            , IEmailService<PasswordRestoreEmailModel> passwordRestoreMailService
            , IEmailService<InviteToCompanyEmailModel> inviteToCompanyMailService
            , IEmailService<InviteToTeamEmailModel> inviteToTeamMailService
            , IEmailService<InviteToProjectEmailModel> inviteToProjectMailService)
        {
            _logger = logger;
            _userConfirmMailService = userConfirmMailService;
            _passwordRestoreMailService = passwordRestoreMailService;
            _inviteToCompanyMailService = inviteToCompanyMailService;
            _inviteToTeamMailService = inviteToTeamMailService;
            _inviteToProjectMailService = inviteToProjectMailService;
            
            _logger.LogInformation("Initialization RabbitMessageHandler success");
        }

        public Task Subscribe(IBus bus)
        {
            bus.Subscribe<UserConfirmEmailModel>(
                subscriptionId: "SquadioEMailListenerRabbitMQ",
                onMessage: async item => { await this.HandleEmailMessage(item); });
            
            bus.Subscribe<PasswordRestoreEmailModel>(
                subscriptionId: "SquadioEMailListenerRabbitMQ",
                onMessage: async item => { await this.HandleEmailMessage(item); });
            
            bus.Subscribe<InviteToCompanyEmailModel>(
                subscriptionId: "SquadioEMailListenerRabbitMQ",
                onMessage: async item => { await this.HandleEmailMessage(item); });
            
            bus.Subscribe<InviteToTeamEmailModel>(
                subscriptionId: "SquadioEMailListenerRabbitMQ",
                onMessage: async item => { await this.HandleEmailMessage(item); });
            
            bus.Subscribe<InviteToProjectEmailModel>(
                subscriptionId: "SquadioEMailListenerRabbitMQ",
                onMessage: async item => { await this.HandleEmailMessage(item); });
            
            _logger.LogInformation("Subscribe success");
            
            return Task.CompletedTask;
        }

        private async Task HandleEmailMessage(UserConfirmEmailModel item)
        {
            await _userConfirmMailService.Send(item);
        }

        private async Task HandleEmailMessage(PasswordRestoreEmailModel item)
        {
            await _passwordRestoreMailService.Send(item);
        }

        private async Task HandleEmailMessage(InviteToCompanyEmailModel item)
        {
            await _inviteToCompanyMailService.Send(item);
        }

        private async Task HandleEmailMessage(InviteToTeamEmailModel item)
        {
            await _inviteToTeamMailService.Send(item);
        }

        private async Task HandleEmailMessage(InviteToProjectEmailModel item)
        {
            await _inviteToProjectMailService.Send(item);
        }
    }
}