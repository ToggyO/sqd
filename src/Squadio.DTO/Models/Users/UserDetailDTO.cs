using System;

namespace Squadio.DTO.Models.Users
{
    public class UserDetailDTO : UserDTO
    {
        public Guid RoleId { get; set; }
    }
}