using System;
using System.ComponentModel.DataAnnotations;

namespace Squadio.DTO.Users
{
    public class UserSetPasswordDTO
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}