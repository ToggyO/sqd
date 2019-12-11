using System.Collections.Generic;

namespace Squadio.DTO.Resources
{
    public class ResourceDTO
    {
        public string OriginalUrl { get; set; }
        public IDictionary<string, string> FormatUrls { get; set; }
    }
}