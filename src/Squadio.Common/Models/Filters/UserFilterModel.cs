using Squadio.Domain.Enums;

namespace Squadio.Common.Models.Filters
{
    public class UserFilterModel
    {
        public UserStatus? UserStatus { get; set; }
        public bool IncludeDeleted { get; set; }
        public bool IncludeAdmin { get; set; }
    }
}