using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Auth;
using Squadio.DTO.Models.Resources;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

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
        Task<Response<UserDTO>> SaveNewAvatar(FormImageCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> SaveNewAvatar(ByteImageCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserDTO>> DeleteAvatar(ClaimsPrincipal claims);
    }
}