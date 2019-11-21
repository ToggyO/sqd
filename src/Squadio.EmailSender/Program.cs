using System;
using EasyNetQ;
using Squadio.EmailSender.Models.Email;

namespace Squadio.EmailSender
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost:5672"))
            {
                bus.Subscribe<UserConfirmEmailModel>("test", HandleTextMessage);
            }
        }

        static void HandleTextMessage(UserConfirmEmailModel textMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got code: {0}, for {1}", textMessage.Code, textMessage.To);
            Console.ResetColor();
        }
    }
}