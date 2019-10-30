using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users
{
    public interface IUsersHandler
    {
        Task<Response<PageModel<UserDTO>>> GetPage(PageModel model);
        Task<Response<UserDTO>> GetCurrentUser(ClaimsPrincipal claims);
        Task<Response<UserDTO>> UpdateCurrentUser(UserUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> GetById(Guid id);
        Task<Response<UserDTO>> SetPassword(UserSetPasswordDTO dto);
        Task<Response> ResetPasswordRequest(string email);
        Task<Response<UserDTO>> DeleteUser(Guid id);
    }
}
