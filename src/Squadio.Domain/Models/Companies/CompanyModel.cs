using System;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;

namespace Squadio.Domain.Models.Companies
{
    public class CompanyModel : BaseModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public Guid CreatorId { get; set; }
        public UserModel Creator { get; set; }
        public CompanyStatus Status { get; set; }
    }
}