using System;

namespace Squadio.Common.Models.Filters
{
    public class ProjectFilter
    {
        public Guid? TeamId { get; set; }
        public Guid? CompanyId { get; set; }
    }
}