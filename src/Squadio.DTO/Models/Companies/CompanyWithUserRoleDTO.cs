using System;
using Squadio.Domain.Enums;

namespace Squadio.DTO.Models.Companies
{
    public class CompanyWithUserRoleDTO
    {
        public Guid CompanyId { get; set; }
        // public CompanyDTO Company { get; set; }
        public string CompanyName { get; set; }
        public MembershipStatus MembershipStatus { get; set; }
    }
}