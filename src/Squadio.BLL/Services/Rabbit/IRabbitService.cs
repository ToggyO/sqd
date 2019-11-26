using System.Threading.Tasks;
using Squadio.Common.Models.Email;

namespace Squadio.BLL.Services.Rabbit
{
    public interface IRabbitService
    {
        Task Send<TEmailModel>(TEmailModel model) where TEmailModel : EmailAbstractModel;
    }
}