using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Squadio.Common.Models.Email;

namespace Squadio.EmailSender.EmailService.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IServiceProvider _serviceProvider;

        public EmailService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public async Task Send<TEmailModel>(TEmailModel model) where TEmailModel : EmailAbstractModel
        {
            await _serviceProvider.GetService<IEmailService<TEmailModel>>().Send(model);
        }
    }
}