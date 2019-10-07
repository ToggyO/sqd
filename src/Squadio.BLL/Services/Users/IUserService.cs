
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Users
{
    public interface IUserService
    {
        Task SignUp(string email);
        Task<AuthInfoDTO> SetPassword(string code, string password);
    }
}
