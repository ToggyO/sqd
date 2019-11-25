using Squadio.Common.Models.Email;

namespace Squadio.EmailSender.RabbitMessageHandler
{
    public interface IRabbitMessageHandler
    {
        void HandleEmailMessage(UserConfirmEmailModel message);
    }
}