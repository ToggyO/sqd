using System;

namespace Squadio.Common.Models.Filters
{
    public class TeamFilter
    {
        public Guid? UserId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? TeamId { get; set; }
    }
}