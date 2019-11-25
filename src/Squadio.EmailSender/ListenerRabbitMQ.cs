using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Rabbit;

namespace Squadio.EmailSender
{
    public class ListenerRabbitMQ : BackgroundService
    {
        private IBus bus { get; set; }
        private readonly ILogger<ListenerRabbitMQ> _logger;
        private readonly IOptions<RabbitConnectionModel> _rabbitConnection;
        
        private const int maxCountTry = 60;

        public ListenerRabbitMQ(ILogger<ListenerRabbitMQ> logger
            , IOptions<RabbitConnectionModel> rabbitConnection)
        {
            _rabbitConnection = rabbitConnection;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            var isRabbitConnected = false;
            _logger.LogInformation("Connection to Rabbit");
            _logger.LogInformation("Connection string: {rabbitConnectionString}", _rabbitConnection.Value.ConnectionString);
            
            var count = 1;

            while (!isRabbitConnected)
            {
                bus = RabbitHutch.CreateBus(_rabbitConnection.Value.ConnectionString);
                
                if (bus.IsConnected)
                {
                    isRabbitConnected = true;
                }

                _logger.LogInformation("Connection to Rabbit ({try})", count);
                count++;
                if (count - 1 >= maxCountTry)
                {
                    isRabbitConnected = true;
                }

                await Task.Delay(1000);
            }

            if (!bus.IsConnected)
            {
                _logger.LogError("Can not connect to Rabbit");
                _logger.LogInformation("Connection string: {rabbitConnectionString}", _rabbitConnection.Value.ConnectionString);
                return;
            }

            _logger.LogInformation("Connection success");

            #region subscriptions
            
            bus.Subscribe<UserConfirmEmailModel>(
                subscriptionId: "SquadioListenerRabbitMQ_b235a412-81b7-42af-908e-9557e88a3237",
                onMessage: MessageHandlers.HandleEmailMessage);

            #endregion
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {}
        }
        
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            bus.Dispose();
            return Task.CompletedTask;
        }
    }
}