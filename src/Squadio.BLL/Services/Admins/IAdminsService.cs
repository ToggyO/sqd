using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Admins
{
    public interface IAdminsService
    {
        Task<Response<UserDTO>> SetPassword(string email, string password);
        Task<Response<UserDTO>> ResetPassword(string code, string password);
        Task<Response> ResetPasswordRequest(string email);
    }
}