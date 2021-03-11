using Squadio.Domain.Enums;

namespace Squadio.DTO.Models.SignUp
{
    public class UserRegistrationStepDTO
    {
        public RegistrationStep Step { get; set; }
        public MembershipStatus MembershipStatus { get; set; }
    }
}