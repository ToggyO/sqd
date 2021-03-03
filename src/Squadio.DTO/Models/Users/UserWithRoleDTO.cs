using System;
using Squadio.Domain.Enums;

namespace Squadio.DTO.Models.Users
{
    public class UserWithRoleDTO
    {
        public Guid UserId { get; set; }
        public UserDTO User { get; set; }
        public MembershipStatus MembershipStatus { get; set; }
        /// <summary>
        /// When user added to entity
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}