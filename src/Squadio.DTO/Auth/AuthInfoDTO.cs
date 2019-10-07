using System;
using Squadio.DTO.Users;

namespace Squadio.DTO.Auth
{
    public class AuthInfoDTO
    {
        public UserDTO User { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpire { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpire { get; set; }
    }
}