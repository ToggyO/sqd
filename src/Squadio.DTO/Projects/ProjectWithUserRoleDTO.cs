using System;

namespace Squadio.DTO.Projects
{
    public class ProjectWithUserRoleDTO
    {
        public Guid ProjectId { get; set; }
        public ProjectDTO Project { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// When user added to project
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}