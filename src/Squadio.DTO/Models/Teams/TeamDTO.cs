using System;
using Squadio.DTO.Models.Users;

namespace Squadio.DTO.Models.Teams
{
    public class TeamDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public UserDTO Creator { get; set; }
        public string ColorHex { get; set; }
    }
}