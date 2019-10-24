using System;
using Squadio.DTO.Users;

namespace Squadio.DTO.Auth
{
    public class AuthInfoDTO
    {
        public UserDTO User { get; set; }
        public UserRegistrationStepDTO RegistrationStep { get; set; }
        public TokenDTO Token { get; set; }
    }
}