using System.ComponentModel.DataAnnotations;

namespace Squadio.DTO.Models.Users.Settings
{
    public class UserSetPasswordDTO
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
    }
}