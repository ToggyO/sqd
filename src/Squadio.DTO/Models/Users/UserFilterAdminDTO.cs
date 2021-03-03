using Squadio.Domain.Enums;

namespace Squadio.DTO.Models.Users
{
    public class UserFilterAdminDTO : UserFilterDTO
    {
        public UserStatus? UserStatus { get; set; }
        public bool IncludeDeleted { get; set; }
        public bool IncludeAdmin { get; set; }
    }
}