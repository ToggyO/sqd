using System.Threading.Tasks;
using Squadio.BLL.Services.Tokens;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;

namespace Squadio.API.Handlers.Auth.Implementation
{
    public class AuthHandler : IAuthHandler
    {
        private readonly ITokensService _tokensService;
        
        public AuthHandler(ITokensService tokensService)
        {
            _tokensService = tokensService;
        }
        
        public async Task<Response<AuthInfoDTO>> Authenticate(CredentialsDTO request)
        {
            var userAuthDTO = await _tokensService.Authenticate(request);
            return new Response<AuthInfoDTO>
            {
                Data = userAuthDTO
            };
        }

        public async Task<Response<TokenDTO>> RefreshToken(string refreshToken)
        {
            var tokenDTO = await _tokensService.RefreshToken(refreshToken);
            return new Response<TokenDTO>
            {
                Data = tokenDTO
            };
        }

        public async Task<Response<AuthInfoDTO>> GoogleAuthenticate(string gmailToken)
        {
            var userAuthDTO = await _tokensService.GoogleAuthenticate(gmailToken);
            return new Response<AuthInfoDTO>
            {
                Data = userAuthDTO
            };
        }
    }
}