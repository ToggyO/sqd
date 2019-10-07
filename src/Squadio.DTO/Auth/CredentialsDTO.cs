using System.ComponentModel.DataAnnotations;

namespace Squadio.DTO.Auth
{
    public class CredentialsDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}