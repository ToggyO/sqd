using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Rabbit;
using Squadio.EmailSender.RabbitMessageHandler;

namespace Squadio.EmailSender
{
    public class ListenerRabbitMQ : BackgroundService
    {
        private IBus bus { get; set; }
        private readonly ILogger<ListenerRabbitMQ> _logger;
        private readonly IOptions<RabbitConnectionModel> _rabbitConnection;
        private readonly IRabbitMessageHandler _rabbitMessageHandler;
        
        private const int tryingSeconds = 60;

        public ListenerRabbitMQ(ILogger<ListenerRabbitMQ> logger
            , IOptions<RabbitConnectionModel> rabbitConnection
            , IRabbitMessageHandler rabbitMessageHandler)
        {
            _logger = logger;
            _rabbitConnection = rabbitConnection;
            _rabbitMessageHandler = rabbitMessageHandler;
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            await RabbitConnection();

            await _rabbitMessageHandler.Subscribe(bus);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {}
            return Task.CompletedTask;
        }
        
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            bus.Dispose();
            return Task.CompletedTask;
        }

        private async Task RabbitConnection()
        {
            var isRabbitConnected = false;
            _logger.LogInformation("Connection to Rabbit: {rabbitConnectionString}", _rabbitConnection.Value.ConnectionString);
            
            var count = 1;
            
            bus = RabbitHutch.CreateBus(_rabbitConnection.Value.ConnectionString);

            while (!isRabbitConnected)
            {
                
                if (bus.IsConnected || count - 1 >= tryingSeconds)
                {
                    isRabbitConnected = true;
                }
                
                count++;

                await Task.Delay(1000);
            }

            if (!bus.IsConnected)
            {
                _logger.LogError("Can not connect to Rabbit: {rabbitConnectionString}", _rabbitConnection.Value.ConnectionString);
                throw new Exception($"Can not connect to Rabbit");
            }

            _logger.LogInformation("Connection success");
        }
    }
}