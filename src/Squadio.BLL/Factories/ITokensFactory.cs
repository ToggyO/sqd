using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Auth;

namespace Squadio.BLL.Factories
{
    public interface ITokensFactory
    {
        Task<SimpleTokenDTO> CreateCustomToken(int lifeTime, string tokenName, Dictionary<string, string> claims = null);
        TokenStatus ValidateCustomToken(string token, string tokenName);
        Task<TokenDTO> CreateToken(UserModel user);
        TokenStatus ValidateToken(string token, out ClaimsPrincipal principal);
    }
}