using Squadio.Domain.Enums;
using Squadio.DTO.Models.Users;

namespace Squadio.DTO.Models.Admin
{
    public class UserFilterAdminDTO : UserFilterDTO
    {
        public UserStatus? UserStatus { get; set; }
        public bool IncludeDeleted { get; set; }
        public bool IncludeAdmin { get; set; }
    }
}