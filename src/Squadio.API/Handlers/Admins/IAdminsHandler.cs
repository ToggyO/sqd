using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;
using Squadio.DTO.Users.Settings;

namespace Squadio.API.Handlers.Admins
{
    public interface IAdminsHandler
    {
        Task<Response<PageModel<UserDTO>>> GetUsersPage(PageModel model, string search);
        Task<Response> ChangePassword(UserSetPasswordDTO dto, ClaimsPrincipal claims);
        Task<Response> ResetPasswordRequest(string email);
        Task<Response<UserDTO>> ResetPassword(UserResetPasswordDTO dto);
    }
}