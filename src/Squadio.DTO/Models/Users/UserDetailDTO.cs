using System;
using Squadio.Domain.Enums;

namespace Squadio.DTO.Models.Users
{
    public class UserDetailDTO : UserDTO
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