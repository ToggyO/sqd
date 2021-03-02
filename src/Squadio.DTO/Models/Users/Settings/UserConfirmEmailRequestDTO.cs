using System;

namespace Squadio.DTO.Models.Users.Settings
{
    public class UserConfirmEmailRequestDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserDTO User { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Code { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}