using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;

namespace Squadio.API.Handlers.Auth
{
    public interface IAuthHandler
    {
        Task<Response<AuthInfoDTO>> Authenticate(CredentialsDTO request);
        Task<Response<TokenDTO>> RefreshToken(string refreshToken);
    }
}