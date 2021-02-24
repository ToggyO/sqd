using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;
using Squadio.DTO.Users.Settings;

namespace Squadio.API.Handlers.Admins
{
    public interface IAdminsHandler
    {
        Task<Response<PageModel<UserDTO>>> GetUsersPage(PageModel model);
        Task<Response> ChangePassword(UserSetPasswordDTO dto, ClaimsPrincipal claims);
        Task<Response> ResetPasswordRequest(string email);
        Task<Response<UserDTO>> ResetPassword(UserResetPasswordDTO dto);
        Task<Response<AuthInfoDTO>> Authenticate(CredentialsDTO request);
    }
}