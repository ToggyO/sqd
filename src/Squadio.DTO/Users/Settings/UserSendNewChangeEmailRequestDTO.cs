
using System.ComponentModel.DataAnnotations;

namespace Squadio.DTO.Users.Settings
{
    public class UserSendNewChangeEmailRequestDTO
    {
        [Required]
        public string NewEmail { get; set; }
        [Required]
        public string Token { get; set; }
    }
}