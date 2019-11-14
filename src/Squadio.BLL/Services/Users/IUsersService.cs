using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Users
{
    public interface IUsersService
    {
        Task<Response<UserDTO>> SetPassword(string email, string password);
        Task<Response<UserDTO>> SetPasswordUsingCode(string code, string password);
        Task<Response> ResetPasswordRequest(string email);
        Task<Response<UserDTO>> UpdateUser(Guid id, UserUpdateDTO dto);
        Task<Response<UserDTO>> DeleteUser(Guid id);
        Task<Response> ChangeEmailRequest(Guid id, ChangeEmailDTO dto);
    }
}
