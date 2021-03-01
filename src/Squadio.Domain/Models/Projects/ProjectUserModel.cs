using System;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;

namespace Squadio.Domain.Models.Projects
{
    public class ProjectUserModel : BaseModel
    {
        public Guid ProjectId { get; set; }
        public ProjectModel Project { get; set; }
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
        public MembershipStatus Status { get; set; }
    }
}