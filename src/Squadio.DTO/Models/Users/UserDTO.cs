using System;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Resources;

namespace Squadio.DTO.Models.Users
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public UserStatus Status { get; set; }
        public ResourceImageDTO Avatar { get; set; }
    }
}