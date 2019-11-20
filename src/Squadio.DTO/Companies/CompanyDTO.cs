using System;
using Squadio.DTO.Users;

namespace Squadio.DTO.Companies
{
    public class CompanyDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserDTO Creator { get; set; }
    }
}