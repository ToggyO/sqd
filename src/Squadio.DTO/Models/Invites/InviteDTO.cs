using System;
using Squadio.Domain.Enums;

namespace Squadio.DTO.Models.Invites
{
    public class InviteDTO : InviteSimpleDTO
    {
        public string Code { get; set; }
        public bool IsDeleted { get; set; }
        public Guid EntityId { get; set; }
        public InviteEntityType EntityType { get; set; }
    }
}