using System;
using Squadio.Domain.Models.Users;

namespace Squadio.Domain.Models.Companies
{
    public class CompanyModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Address { get; set; }
        public Guid CreatorId { get; set; }
        public UserModel Creator { get; set; }
    }
}