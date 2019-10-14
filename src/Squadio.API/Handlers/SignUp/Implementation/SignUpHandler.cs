using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.SignUp;
using Squadio.BLL.Services.SignUp;
using Squadio.BLL.Services.Tokens;
using Squadio.Common.Exceptions.PermissionException;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.SignUp.Implementation
{
    public class SignUpHandler : ISignUpHandler
    {
        private readonly ISignUpProvider _provider;
        private readonly ISignUpService _service;
        private readonly ITokensService _tokensService;
        public SignUpHandler(ISignUpProvider provider
            , ISignUpService service
            , ITokensService tokensService)
        {
            _provider = provider;
            _service = service;
            _tokensService = tokensService;
        }
        

        public async Task<Response> SignUp(string email)
        {
            await _service.SignUp(email);
            var result = new Response();
            return result;
        }

        public async Task<Response<UserDTO>> SignUpGoogle(string googleToken)
        {
            var user = await _service.SignUpGoogle(googleToken);
            var result = new Response<UserDTO>
            {
                Data = user
            };
            return result;
        }

        public async Task<Response<AuthInfoDTO>> SignUpPassword(UserSetPasswordDTO dto)
        {
            await _service.SignUpPassword(dto.Email, dto.Code, dto.Password);
            var tokenUser = await _tokensService.Authenticate(new CredentialsDTO
            {
                Password = dto.Password,
                Email = dto.Email
            });
            var result = new Response<AuthInfoDTO>
            {
                Data = tokenUser
            };
            return result;
        }

        public async Task<Response<UserDTO>> SignUpUsername(UserUpdateDTO dto, ClaimsPrincipal claims)
        {
            var user = await _service.SignUpUsername(claims.GetUserId() ?? throw new PermissionException(), dto);
            var result = new Response<UserDTO>()
            {
                Data = user
            };
            return result;
        }

        public async Task<Response<UserRegistrationStepDTO>> GetRegistrationStep(string email)
        {
            var item = await _provider.GetRegistrationStep(email);
            var result = new Response<UserRegistrationStepDTO>
            {
                Data = item
            };
            return result;
        }
    }
}