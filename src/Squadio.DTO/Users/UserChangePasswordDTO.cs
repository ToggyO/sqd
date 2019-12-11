using System.ComponentModel.DataAnnotations;

namespace Squadio.DTO.Users
{
    public class UserChangePasswordDTO
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string Password { get; set; }
    }
}