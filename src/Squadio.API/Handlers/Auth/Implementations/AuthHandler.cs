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
            var result = await _tokensService.Authenticate(request);
            return result;
        }

        public async Task<Response<TokenDTO>> RefreshToken(string refreshToken)
        {
            var result = await _tokensService.RefreshToken(refreshToken);
            return result;
        }
    }
}