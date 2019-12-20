using System;
using System.Collections.Generic;
using Squadio.DTO.Users;

namespace Squadio.DTO.Companies
{
    public class CompanyListDTO
    {
        public CompanyDTO Company { get; set; }
        public int UsersCount { get; set; }
        public IEnumerable<UserWithRoleDTO> Admins { get; set; }
    }
}