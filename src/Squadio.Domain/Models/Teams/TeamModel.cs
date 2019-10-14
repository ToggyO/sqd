using System;
using Squadio.Domain.Models.Companies;

namespace Squadio.Domain.Models.Teams
{
    public class TeamModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
        public CompanyModel Company { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}