using System;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;

namespace Squadio.Domain.Models.Teams
{
    public class TeamModel : BaseModel
    {
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
        public CompanyModel Company { get; set; }
        public string ColorHex { get; set; }
        public Guid CreatorId { get; set; }
        public UserModel Creator { get; set; }
    }
}