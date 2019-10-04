using System.Threading.Tasks;

namespace Squadio.BLL.Services.Email
{
    public interface IMailService<in T>
    {
        Task Send(T model);
    }
}