using System.Threading.Tasks;
using Squadio.Common.Models.Email;

namespace Squadio.EmailSender.EmailService
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