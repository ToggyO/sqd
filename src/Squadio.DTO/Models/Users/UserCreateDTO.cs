using Squadio.Domain.Enums;

namespace Squadio.DTO.Models.Users
{
    public class UserCreateDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public UserStatus UserStatus { get; set; }
        public SignUpType SignUpBy { get; set; }
        public RegistrationStep Step { get; set; }
        public MembershipStatus MembershipStatus { get; set; }
    }
}