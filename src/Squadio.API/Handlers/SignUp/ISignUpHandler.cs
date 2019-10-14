using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.SignUp
{
    public interface ISignUpHandler
    {
        Task<Response> SignUp(string email);
        Task<Response<AuthInfoDTO>> SignUpPassword(UserSetPasswordDTO dto);
        Task<Response<UserDTO>> SignUpUsername(UserUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserRegistrationStepDTO>> GetRegistrationStep(string email);
    }
}