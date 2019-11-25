using System.Threading.Tasks;
using EasyNetQ;
using Squadio.Common.Models.Email;

namespace Squadio.EmailSender.RabbitMessageHandler
{
    public interface IRabbitMessageHandler
    {
        Task Subscribe(IBus bus);
    }
}