using System;
using Squadio.DTO.Resources;

namespace Squadio.DTO.Users
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ResourceImageDTO Avatar { get; set; }
    }
}