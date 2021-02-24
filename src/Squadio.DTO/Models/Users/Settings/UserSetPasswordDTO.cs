using System.ComponentModel.DataAnnotations;

namespace Squadio.DTO.Models.Users.Settings
{
    public class UserSetPasswordDTO
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string Password { get; set; }
    }
}