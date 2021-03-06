using System;
using Squadio.Domain.Enums;

namespace Squadio.Domain.Models.Users
{
    public class UserRegistrationStepModel : BaseModel
    {
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
        public RegistrationStep Step { get; set; }
        public MembershipStatus Status { get; set; }
    }
}