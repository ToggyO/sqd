using System;
using Squadio.Domain.Models.Users;

namespace Squadio.Domain.Models.Companies
{
    public class CompanyModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid AdministratorId { get; set; }
        public UserModel Administrator { get; set; }
    }
}