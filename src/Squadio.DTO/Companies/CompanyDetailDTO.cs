using System;
using System.Collections.Generic;
using Squadio.DTO.Users;

namespace Squadio.DTO.Companies
{
    public class CompanyDetailDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserDTO Creator { get; set; }
        public int UsersCount { get; set; }
        public IEnumerable<UserWithRoleDTO> Admins { get; set; }
    }
}