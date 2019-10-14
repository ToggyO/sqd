using System.Threading.Tasks;
using Squadio.DTO.Auth;

namespace Squadio.BLL.Services.Tokens
{
    public interface ITokensService
    {
        Task<AuthInfoDTO> Authenticate(CredentialsDTO dto);
        Task<TokenDTO> RefreshToken(string refreshToken);
        Task<AuthInfoDTO> GoogleAuthenticate(string googleToken);
    }
}