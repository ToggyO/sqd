using System;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;

namespace Squadio.Domain.Models.Companies.Administrators
{
    public class CompanyAdministrator
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public CompanyModel Company { get; set; }
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
    }
}