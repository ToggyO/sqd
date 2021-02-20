using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Squadio.Domain.Enums;

namespace Squadio.Common.Models.Filters
{
    public class UserWithCompaniesFilter
    {
        public MembershipStatus? Status { get; set; } = null;
    }
}