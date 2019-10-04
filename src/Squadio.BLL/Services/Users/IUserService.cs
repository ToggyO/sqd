
using System.Threading.Tasks;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Users
{
    public interface IUserService
    {
        Task SignUp(string email);
    }
}
