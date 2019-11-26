using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Rabbit;

namespace Squadio.BLL.Services.Rabbit.Publisher.Implementation
{
    public class RabbitPublisher : IRabbitPublisher
    {
        private readonly ILogger _logger;
        private readonly IOptions<RabbitConnectionModel> _rabbitConnections;

        public RabbitPublisher(ILogger<RabbitPublisher> logger
            , IOptions<RabbitConnectionModel> rabbitConnections)
        {
            _logger = logger;
            _rabbitConnections = rabbitConnections;
        }

        public void Send<TEmailModel>(TEmailModel model) where TEmailModel : EmailAbstractModel
        {
            int max = 3;
            int count = 0;
            using (var bus = RabbitHutch.CreateBus(_rabbitConnections.Value.ConnectionString))
            {
                while (!bus.IsConnected && count <= max)
                {
                    count++;
                    Task.Delay(1000).Wait();
                }
                if (bus.IsConnected)
                {
                    bus.Publish(model); 
                }
                else
                {
                    _logger.LogWarning("Can't push model to Rabbit cause connection fault");
                }
            }
        }

        public async Task SendAsync<TEmailModel>(TEmailModel model) where TEmailModel : EmailAbstractModel
        {
            int max = 5;
            int count = 0;
            using (var bus = RabbitHutch.CreateBus(_rabbitConnections.Value.ConnectionString))
            {
                while (!bus.IsConnected && count <= max)
                {
                    count++;
                    await Task.Delay(1000);
                }
                if (bus.IsConnected)
                {
                    await bus.PublishAsync(model); 
                }
                else
                {
                    _logger.LogWarning("Can't push model to Rabbit cause connection fault");
                }
            }
        }
    }
}