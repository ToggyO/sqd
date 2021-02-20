using Squadio.Domain.Enums;

namespace Squadio.DTO.Users
{
    public class UserCreateDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public UserStatus UserStatus { get; set; }
        public SignUpType SignUpBy { get; set; }
    }
}