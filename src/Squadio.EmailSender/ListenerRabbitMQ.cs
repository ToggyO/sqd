using System;
using System.Threading.Tasks;
using EasyNetQ;
using Squadio.Common.Models.Email;

namespace Squadio.EmailSender
{
    public class ListenerRabbitMQ
    {
        // for docker
        private const string RabbitConnectionString = "host=rabbit-dev;username=rabbitmq;password=rabbitmq";
        // for debug
        //private const string RabbitConnectionString = "host=localhost;username=rabbitmq;password=rabbitmq";
        
        private const int maxCountTry = 10;

        public ListenerRabbitMQ()
        {
        }

        public async Task Execute()
        {
            var isRabbitConnected = false;

            using (var bus = RabbitHutch.CreateBus(RabbitConnectionString))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("[{0}] Connection to Rabbit", DateTime.UtcNow);
                Console.ResetColor();
                var count = 1;
                while (!isRabbitConnected)
                {
                    if (bus.IsConnected)
                    {
                        isRabbitConnected = true;
                    }

                    Console.WriteLine("[{0}] Connection to Rabbit ({1})", DateTime.UtcNow, count);
                    count++;
                    if (count - 1 >= maxCountTry)
                    {
                        isRabbitConnected = true;
                    }

                    await Task.Delay(3000);
                }

                if (!bus.IsConnected)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("[{0}] Can not connect to Rabbit", DateTime.UtcNow);
                    Console.ResetColor();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("[{0}] Connection success", DateTime.UtcNow);
                Console.ResetColor();

                try
                {
                    bus.Subscribe<UserConfirmEmailModel>(
                        subscriptionId: "SquadioListenerRabbitMQ_e0ee4bac-5adb-4eee-9b7c-29c9a481894b",
                        onMessage: HandleEmailMessage);
                }
                catch
                {
                    Console.WriteLine("Can't subscribe RabbitMQ");
                }

                while (true){}
            }
        }

        private static void HandleEmailMessage(UserConfirmEmailModel message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got code: {0}, for {1}", message.Code, message.To);
            Console.ResetColor();
        }
    }
}