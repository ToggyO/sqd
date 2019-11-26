using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Squadio.BLL.Services.Rabbit.Publisher;
using Squadio.Common.Models.Email;

namespace Squadio.BLL.Services.Rabbit.Implementations
{
    public class RabbitService : IRabbitService
    {
        private readonly IRabbitPublisher _rabbitPublisher;

        public RabbitService(IRabbitPublisher rabbitPublisher)
        {
            _rabbitPublisher = rabbitPublisher;
        }
        
        public async Task Send<TEmailModel>(TEmailModel model) where TEmailModel : EmailAbstractModel
        {
            await _rabbitPublisher.SendAsync<TEmailModel>(model);
        }
    }
}