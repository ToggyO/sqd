using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;

namespace Squadio.BLL.Services.Tokens
{
    public interface ITokensService
    {
        Task<Response<AuthInfoDTO>> Authenticate(CredentialsDTO dto);
        Task<Response<TokenDTO>> RefreshToken(string refreshToken);
        Task<Response<AuthInfoDTO>> GoogleAuthenticate(string googleToken);
    }
}