using System;
using Squadio.DTO.Users;

namespace Squadio.DTO.Teams
{
    public class TeamDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public UserDTO Creator { get; set; }
    }
}