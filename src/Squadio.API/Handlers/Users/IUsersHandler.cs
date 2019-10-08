using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users
{
    public interface IUsersHandler
    {
        Task<Response<UserDTO>> GetById(Guid id);
        Task<Response<UserDTO>> SetPassword(UserSetPasswordDTO dto);
        Task<Response> SignUp(string email);
        Task<Response> ResetPasswordRequest(string email);
    }
}
