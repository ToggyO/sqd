using System.ComponentModel.DataAnnotations;

namespace Squadio.DTO.Models.Users.Settings
{
    public class UserChangeEmailRequestDTO
    {
        [Required]
        public string NewEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}