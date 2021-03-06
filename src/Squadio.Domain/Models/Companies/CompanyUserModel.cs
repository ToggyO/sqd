using System;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;

namespace Squadio.Domain.Models.Companies
{
    public class CompanyUserModel : BaseModel
    {
        public Guid CompanyId { get; set; }
        public CompanyModel Company { get; set; }
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
        public MembershipStatus Status { get; set; }
    }
}