using Squadio.Domain.Enums;

namespace Squadio.DTO.Models.Users
{
    public class UserAdminFilterDTO : UserFilterDTO
    {
        //TODO: filter by membership status in companies
        public UserStatus? UserStatus { get; set; }
        public bool IncludeDeleted { get; set; }
        public bool IncludeAdmin { get; set; }
    }
}