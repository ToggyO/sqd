using System.Threading.Tasks;

namespace Squadio.EmailSender
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var listener = new ListenerRabbitMQ();
            await listener.Execute();
        }
    }
}