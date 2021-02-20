using System;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;

namespace Squadio.Domain.Models.Projects
{
    public class ProjectModel : BaseModel
    {
        public string Name { get; set; }
        public Guid TeamId { get; set; }
        public TeamModel Team { get; set; }
        public string ColorHex { get; set; }
        public Guid CreatorId { get; set; }
        public UserModel Creator { get; set; }
    }
}