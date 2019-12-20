using System;

namespace Squadio.DTO.Teams
{
    public class TeamWithUserRoleDTO
    {
        public Guid TeamId { get; set; }
        public TeamDTO Team { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// When user added to team
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}