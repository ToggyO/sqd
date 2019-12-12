using System;
using System.Collections.Generic;

namespace Squadio.DTO.Resources
{
    public class ResourceImageDTO
    {
        public Guid ResourceId { get; set; }
        public string OriginalUrl { get; set; }
        public IDictionary<string, string> FormatUrls { get; set; }
    }
}