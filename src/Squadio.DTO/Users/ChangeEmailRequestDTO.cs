using System.ComponentModel.DataAnnotations;

namespace Squadio.DTO.Users
{
    public class ChangeEmailRequestDTO
    {
        [Required]
        public string NewEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}