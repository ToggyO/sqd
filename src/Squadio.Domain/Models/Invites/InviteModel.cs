using System;

namespace Squadio.Domain.Models.Invites
{
    public class InviteModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Activated { get; set; }
    }
}