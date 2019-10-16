using System;

namespace Squadio.DTO.Invites
{
    public class InviteDTO
    {
        public Guid? Id { get; set; }
        public string Email { get; set; }
        public bool IsSent { get; set; }
    }
}