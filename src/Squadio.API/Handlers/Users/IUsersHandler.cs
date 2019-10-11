using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users
{
    public interface IUsersHandler
    {
        Task<Response<IEnumerable<UserDTO>>> GetAll();
        Task<Response<UserDTO>> GetCurrentUser(ClaimsPrincipal claims);
        Task<Response<UserDTO>> UpdateCurrentUser(UserUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> GetById(Guid id);
        Task<Response<UserDTO>> SetPassword(UserSetPasswordDTO dto);
        Task<Response> SignUp(string email);
        Task<Response> ResetPasswordRequest(string email);
    }
}
