using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Squadio.Common.Enums;
using Squadio.Common.Settings;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Auth;

namespace Squadio.BLL.Factories.Implementations
{
    public class TokensFactory : ITokensFactory
    {
        private readonly IOptions<ApiSettings> _settings;

        public TokensFactory(IOptions<ApiSettings> settings)
        {
            _settings = settings;
        }

        public Task<SimpleTokenDTO> CreateCustomToken(int lifeTime, string tokenName, Dictionary<string, string> claims = null)
        {
            var exp = DateTime.UtcNow;
            exp = exp.AddMinutes(lifeTime);
            var tempClaims = new List<Claim>();
            if (claims != null)
            {
                tempClaims.AddRange(claims.Select(claim => new Claim(claim.Key, claim.Value)));
            }
            var upperTokenName = tokenName.ToUpper();
            var token = new JwtSecurityToken(
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.PublicKey)),
                    SecurityAlgorithms.HmacSha256),
                issuer: upperTokenName,
                audience: upperTokenName,
                expires: exp,
                claims: tempClaims);
            
            var result = new SimpleTokenDTO
            {
                Token = EncodeToken(token)
            };
            
            return Task.FromResult(result);
        }

        public TokenStatus ValidateCustomToken(string token, string tokenName)
        {
            try
            {
                var principal = new JwtSecurityTokenHandler().ValidateToken(token, GetCustomTokenValidationParameters(tokenName), out var securityToken);
                
                if (DateTime.UtcNow > securityToken.ValidTo)
                    return TokenStatus.Expired;

                var isValid = (securityToken is JwtSecurityToken jwtSecurityToken &&
                               jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase));
                
                return isValid ? TokenStatus.Valid : TokenStatus.Invalid;
            }
            catch
            {
                return TokenStatus.Invalid;
            }
        }

        public Task<TokenDTO> CreateToken(UserModel user)
        {
            var now = DateTime.UtcNow;
            var accessTokenExpire = now.AddMinutes(_settings.Value.AccessTokenExpiresInMinutes);
            var refreshTokenExpire = now.AddMinutes(_settings.Value.RefreshTokenExpiresInMinutes);

            var result = new TokenDTO()
            {
                AccessToken = EncodeToken(GetJwtAccessToken(user, accessTokenExpire)),
                AccessTokenExpire = accessTokenExpire,

                RefreshToken = EncodeToken(GetJwtRefreshToken(user, refreshTokenExpire)),
                RefreshTokenExpire = refreshTokenExpire
            };

            return Task.FromResult(result);
        }

        public TokenStatus ValidateToken(string token, out ClaimsPrincipal principal)
        {
            try
            {
                principal = new JwtSecurityTokenHandler().ValidateToken(token, GetTokenValidationParameters(), out var securityToken);
                
                if (DateTime.UtcNow > securityToken.ValidTo)
                    return TokenStatus.Expired;

                var isValid = (securityToken is JwtSecurityToken jwtSecurityToken &&
                        jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase));
                
                return isValid ? TokenStatus.Valid : TokenStatus.Invalid;
            }
            catch
            {
                principal = null;
                return TokenStatus.Invalid;
            }
        }

        private JwtSecurityToken GetJwtAccessToken(UserModel user, DateTime exp)
        {
            return new JwtSecurityToken(
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.PublicKey)),
                    SecurityAlgorithms.HmacSha256),
                issuer: _settings.Value.ISSUER,
                audience: _settings.Value.AUDIENCE,
                expires: exp,
                claims: new[]
                {
                    new Claim(Common.Extensions.ClaimTypes.UserId, user.Id.ToString("N")),
                    new Claim(Common.Extensions.ClaimTypes.RoleId, user.RoleId.ToString("N")),
                });
        }

        private JwtSecurityToken GetJwtRefreshToken(UserModel user, DateTime exp)
        {
            return new JwtSecurityToken(
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.PublicKey)),
                    SecurityAlgorithms.HmacSha256),
                issuer: _settings.Value.ISSUER,
                audience: _settings.Value.AUDIENCE,
                expires: exp,
                claims: new[]
                {
                    new Claim(Common.Extensions.ClaimTypes.UserId, user.Id.ToString("N")),
                });
        }

        private string EncodeToken(JwtSecurityToken token)
        {
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _settings.Value.ISSUER,
                
                ValidateAudience = true,
                ValidAudience = _settings.Value.AUDIENCE,
                
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_settings.Value.PublicKeyBytes),
                
                ValidateLifetime = false
            };
        }
        private TokenValidationParameters GetCustomTokenValidationParameters(string tokenName)
        {
            var upperTokenName = tokenName.ToUpper();
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = upperTokenName,
                
                ValidateAudience = true,
                ValidAudience = upperTokenName,
                
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_settings.Value.PublicKeyBytes),
                
                ValidateLifetime = false
            };
        }
    }
}