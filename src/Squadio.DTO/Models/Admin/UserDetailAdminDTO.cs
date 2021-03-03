using System;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Users;

namespace Squadio.DTO.Models.Admin
{
    public class UserDetailAdminDTO : UserDTO
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public UIThemeType UITheme { get; set; }
        public SignUpType SignUpType { get; set; }
    }
}