using System;
using Squadio.Domain.Enums;

namespace Squadio.Domain.Models.Users
{
    public class UserRegistrationStepModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
        public RegistrationStep Step { get; set; }
        public DateTime UpdatedDate { get; set; }
        public UserStatus Status { get; set; }
    }
}