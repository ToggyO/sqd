using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Resources;
using Squadio.DTO.Users;
using Squadio.DTO.Users.Settings;

namespace Squadio.API.Handlers.Users
{
    public interface IUsersSettingsHandler
    {
        Task<Response<UserDTO>> UpdateCurrentUser(UserUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> ResetPassword(UserResetPasswordDTO dto);
        Task<Response> ValidateCode(string code);
        Task<Response> SetPassword(UserSetPasswordDTO dto, ClaimsPrincipal claims);
        Task<Response> ResetPasswordRequest(string email);
        Task<Response<SimpleTokenDTO>> ChangeEmailRequest(UserChangeEmailRequestDTO dto, ClaimsPrincipal claims);
        Task<Response> SendNewChangeEmailRequest(UserSendNewChangeEmailRequestDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> SetEmail(UserSetEmailDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> SaveNewAvatar(FileImageCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> SaveNewAvatar(ResourceImageCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> DeleteAvatar(ClaimsPrincipal claims);
    }
}