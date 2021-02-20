using System;
using System.Collections.Generic;
using System.Text;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Resources;
using Squadio.Domain.Models.Roles;

namespace Squadio.Domain.Models.Users
{
    public class UserModel : BaseModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public Guid RoleId { get; set; }
        public RoleModel Role { get; set; }
        public UIThemeType UITheme { get; set; }
        public SignUpType SignUpType { get; set; }
        public UserStatus Status { get; set; }
        public Guid? AvatarId { get; set; }
        public ResourceModel Avatar { get; set; }
    }
}
