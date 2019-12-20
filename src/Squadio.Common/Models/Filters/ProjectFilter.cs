using System;

namespace Squadio.Common.Models.Filters
{
    public class ProjectFilter
    {
        public Guid? UserId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? TeamId { get; set; }
        public Guid? ProjectId { get; set; }
    }
}