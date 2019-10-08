using System;

namespace Squadio.DTO.Auth
{
    public class TokenDTO
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpire { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpire { get; set; }
    }
}