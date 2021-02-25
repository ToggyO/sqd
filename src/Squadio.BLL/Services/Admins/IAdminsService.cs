using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Services.Admins
{
    public interface IAdminsService
    {
        Task<Response> SetPassword(string email, string password);
        Task<Response> ResetPassword(string code, string password);
        Task<Response> ResetPasswordRequest(string email);
        Task<Response> CreateAdmin(string email, string name, string password);
    }
}