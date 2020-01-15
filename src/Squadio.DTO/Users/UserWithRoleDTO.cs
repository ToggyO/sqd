using System;

namespace Squadio.DTO.Users
{
    public class UserWithRoleDTO
    {
        public Guid UserId { get; set; }
        public UserDTO User { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// When user added to entity
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}