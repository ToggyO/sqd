using System;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;

namespace Squadio.Domain.Models.Invites
{
    public class InviteModel : BaseModel
    {
        public Guid CreatorId { get; set; }
        public UserModel Creator { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentDate { get; set; }
        public Guid EntityId { get; set; }
        public InviteEntityType EntityType { get; set; }
    }
}