using System.ComponentModel.DataAnnotations;

namespace Squadio.DTO.Models.Users.Settings
{
    public class UserResetPasswordDTO
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Password { get; set; }
    }
}