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
        private readonly IEmailService<InviteUserEmailModel> _inviteUserMailService;
        private readonly IEmailService<PasswordRestoreAdminEmailModel> _passwordRestoreAdminMailService;
        private readonly IEmailService<AddUserEmailModel> _addUserMailService;

        public RabbitMessageHandler(ILogger<RabbitMessageHandler> logger
            , IEmailService<UserConfirmEmailModel> userConfirmMailService
            , IEmailService<PasswordRestoreEmailModel> passwordRestoreMailService
            , IEmailService<InviteUserEmailModel> inviteUserMailService
            , IEmailService<PasswordRestoreAdminEmailModel> passwordRestoreAdminMailService
            , IEmailService<AddUserEmailModel> addUserMailService)
        {
            _logger = logger;
            _userConfirmMailService = userConfirmMailService;
            _passwordRestoreMailService = passwordRestoreMailService;
            _inviteUserMailService = inviteUserMailService;
            _passwordRestoreAdminMailService = passwordRestoreAdminMailService;
            _addUserMailService = addUserMailService;
        }

        public Task Subscribe(IBus bus)
        {
            bus.Subscribe<UserConfirmEmailModel>(
                subscriptionId: "SquadioEMailListenerRabbitMQ",
                onMessage: async item => { await this.HandleEmailMessage(item); });
            
            bus.Subscribe<PasswordRestoreEmailModel>(
                subscriptionId: "SquadioEMailListenerRabbitMQ",
                onMessage: async item => { await this.HandleEmailMessage(item); });
            
            bus.Subscribe<InviteUserEmailModel>(
                subscriptionId: "SquadioEMailListenerRabbitMQ",
                onMessage: async item => { await this.HandleEmailMessage(item); });
            
            bus.Subscribe<PasswordRestoreAdminEmailModel>(
                subscriptionId: "SquadioEMailListenerRabbitMQ",
                onMessage: async item => { await this.HandleEmailMessage(item); });
            
            bus.Subscribe<AddUserEmailModel>(
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

        private async Task HandleEmailMessage(InviteUserEmailModel item)
        {
            await _inviteUserMailService.Send(item);
        }

        private async Task HandleEmailMessage(PasswordRestoreAdminEmailModel item)
        {
            await _passwordRestoreAdminMailService.Send(item);
        }

        private async Task HandleEmailMessage(AddUserEmailModel item)
        {
            await _addUserMailService.Send(item);
        }
    }
}