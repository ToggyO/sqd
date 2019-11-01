using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Squadio.Domain.Enums;

namespace Squadio.Common.Models.Filters
{
    public class UserWithCompaniesFilter
    {
        public UserStatus? Status { get; set; } = null;
    }
}