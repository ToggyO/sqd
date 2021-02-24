using System;
using Squadio.Domain.Enums;
using Squadio.DTO.Resources;

namespace Squadio.DTO.Users
{
    public class UserDetailDTO : UserDTO
    {
        public Guid RoleId { get; set; }
    }
}