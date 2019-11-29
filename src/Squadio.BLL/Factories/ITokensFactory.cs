using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Auth;

namespace Squadio.BLL.Factories
{
    public interface ITokensFactory
    {
        Task<TokenDTO> CreateToken(UserModel user);
        /// <summary>
        /// Return true if refresh token is valid
        /// </summary>
        TokenStatus ValidateToken(string token, out ClaimsPrincipal principal);
    }
}