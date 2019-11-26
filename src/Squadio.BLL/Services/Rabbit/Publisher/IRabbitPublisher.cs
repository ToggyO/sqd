using System.Threading.Tasks;
using Squadio.Common.Models.Email;

namespace Squadio.BLL.Services.Rabbit.Publisher
{
    public interface IRabbitPublisher
    {
        void Send<TEmailModel>(TEmailModel model) where TEmailModel : EmailAbstractModel;
        Task SendAsync<TEmailModel>(TEmailModel model) where TEmailModel : EmailAbstractModel;
    }
}