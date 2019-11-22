using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Squadio.Common.Models.Email;

namespace Squadio.EmailSender
{
    public class ListenerRabbitMQ : BackgroundService
    {
        // for docker
        //private string _rabbitConnectionString = "host=rabbit-dev;username=rabbitmq;password=rabbitmq";
        // for debug
        private readonly string _rabbitConnectionString;
        private IBus bus { get; set; }
        private readonly ILogger<ListenerRabbitMQ> _logger;
        
        private const int maxCountTry = 60;

        public ListenerRabbitMQ(ILogger<ListenerRabbitMQ> logger)
        {
            // for debug
            _rabbitConnectionString = "host=localhost;username=rabbitmq;password=rabbitmq";
            // for docker
            //_rabbitConnectionString = "host=rabbit-dev;username=rabbitmq;password=rabbitmq";
            
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            var isRabbitConnected = false;
            _logger.LogInformation("Connection to Rabbit");
            var count = 1;

            while (!isRabbitConnected)
            {
                bus = RabbitHutch.CreateBus(_rabbitConnectionString);
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