using System.Threading.Tasks;
using Squadio.API.Handlers.SignUp;
using Squadio.BLL.Services.Tokens;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;

namespace Squadio.API.Handlers.Auth.Implementation
{
    public class AuthHandler : IAuthHandler
    {
        private readonly ITokensService _tokensService;
        private readonly ISignUpHandler _signUpHandler;
        
        public AuthHandler(ITokensService tokensService
            , ISignUpHandler signUpHandler)
        {
            _tokensService = tokensService;
            _signUpHandler = signUpHandler;
        }
        
        public async Task<Response<AuthInfoDTO>> Authenticate(CredentialsDTO request)
        {
            var result = await _tokensService.Authenticate(request);
            return result;
        }

        public async Task<Response<TokenDTO>> RefreshToken(string refreshToken)
        {
            var result = await _tokensService.RefreshToken(refreshToken);
            return result;
        }

        public async Task<Response<AuthInfoDTO>> GoogleAuthenticate(string googleToken)
        {
            var result = await _tokensService.GoogleAuthenticate(googleToken);
            if (!result.IsSuccess)
            {
                return await _signUpHandler.SignUpGoogle(googleToken);
            }
            return result;
        }
    }
}