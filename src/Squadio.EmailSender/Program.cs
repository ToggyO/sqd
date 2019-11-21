using System;
using EasyNetQ;
using Squadio.EmailSender.Models.Email;

namespace Squadio.EmailSender
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = 0;
            Console.WriteLine("-------------------------");
            Console.WriteLine("[" + DateTime.Now+ "]");
            Console.WriteLine("-------------------------");
            Console.WriteLine("Mail sender ololo");
            using (var bus = RabbitHutch.CreateBus("host=localhost:5672"))
            {
                try
                {
                    bus.Subscribe<UserConfirmEmailModel>("test", HandleTextMessage);
                }
                catch
                {
                    Console.WriteLine("Error RabbitMQ");
                }
            }
            Console.WriteLine("Mail sender DONE");
        }

        static void HandleTextMessage(UserConfirmEmailModel textMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got code: {0}, for {1}", textMessage.Code, textMessage.To);
            Console.ResetColor();
        }
    }
}