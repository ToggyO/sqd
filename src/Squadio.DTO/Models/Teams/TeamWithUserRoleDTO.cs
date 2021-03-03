using System;
using Squadio.Domain.Enums;

namespace Squadio.DTO.Models.Teams
{
    public class TeamWithUserRoleDTO
    {
        public Guid TeamId { get; set; }
        // public TeamDTO Team { get; set; }
        public MembershipStatus MembershipStatus { get; set; }
        /// <summary>
        /// When user added to team
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}