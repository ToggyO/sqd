using System.Collections.Generic;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Users;

namespace Squadio.DTO.Models.Admin
{
    public class CompanyDetailAdminDTO : CompanyDTO
    {
        public int UsersCount { get; set; }
        public CompanyStatus Status { get; set; }
        public IEnumerable<UserWithRoleDTO> Admins { get; set; }
    }
}