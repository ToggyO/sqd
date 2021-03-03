using System;
using Squadio.DTO.Models.Users;

namespace Squadio.DTO.Models.Companies
{
    public class CompanyDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatorId { get; set; }
        public UserDTO Creator { get; set; }
    }
}