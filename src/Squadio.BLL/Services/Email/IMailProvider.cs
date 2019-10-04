using System.Threading.Tasks;
using Squadio.Common.Models.Email;

namespace Squadio.BLL.Services.Email
{
    public interface IMailProvider<in T>
    {
        Task<MailMessage> Get(T model);
    }
}