using System.Collections.Generic;
using Squadio.DTO.Models.Users;

namespace Squadio.DTO.Models.Companies
{
    public class CompanyListDTO
    {
        public CompanyDTO Company { get; set; }
        public int UsersCount { get; set; }
        public IEnumerable<UserWithRoleDTO> Admins { get; set; }
    }
}