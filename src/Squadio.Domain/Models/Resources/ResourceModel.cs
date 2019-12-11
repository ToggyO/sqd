using System;

namespace Squadio.Domain.Models.Resources
{
    public class ResourceModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Group { get; set; }
    }
}