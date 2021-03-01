using System;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;

namespace Squadio.Domain.Models.Teams
{
    public class TeamUserModel : BaseModel
    {
        public Guid TeamId { get; set; }
        public TeamModel Team { get; set; }
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
        public MembershipStatus Status { get; set; }
    }
}