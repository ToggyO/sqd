using System;
using Squadio.Domain.Enums;

namespace Squadio.DTO.Models.Projects
{
    public class ProjectWithUserRoleDTO
    {
        public Guid ProjectId { get; set; }
        // public ProjectDTO Project { get; set; }
        public MembershipStatus MembershipStatus { get; set; }
        /// <summary>
        /// When user added to project
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}