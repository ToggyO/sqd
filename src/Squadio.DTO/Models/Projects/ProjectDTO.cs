using System;
using Squadio.DTO.Models.Users;

namespace Squadio.DTO.Models.Projects
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public UserDTO Creator { get; set; }
        public Guid TeamId { get; set; }
        public string ColorHex { get; set; }
    }
}