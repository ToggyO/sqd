using System;
using Squadio.DTO.Users;

namespace Squadio.DTO.Teams
{
    public class TeamUserDTO
    {
        public Guid TeamId { get; set; }
        public TeamDTO Team { get; set; }
        public Guid UserId { get; set; }
        public UserDTO User { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// When user added to team
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}