using Squadio.DTO.Models.Users;

namespace Squadio.DTO.Models.Auth
{
    public class AuthInfoDTO
    {
        public UserDTO User { get; set; }
        public TokenDTO Token { get; set; }
    }
}