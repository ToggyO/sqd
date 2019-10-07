using System.Threading.Tasks;
using Squadio.Common.Models.Email;

namespace Squadio.BLL.Services.Email
{
    public interface IEmailService<TEmailModel> where TEmailModel : EmailAbstractModel
    {
        Task Send(TEmailModel emailModel);
    }

    public interface IEmailService
    {
        Task Send<TEmailModel>(TEmailModel model) where TEmailModel : EmailAbstractModel;
    }
}