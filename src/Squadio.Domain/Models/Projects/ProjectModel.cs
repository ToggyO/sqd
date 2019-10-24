using System;
using Squadio.Domain.Models.Companies;

namespace Squadio.Domain.Models.Projects
{
    public class ProjectModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
        public CompanyModel Company { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ColorHex { get; set; }
    }
}