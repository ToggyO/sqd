using System.Collections.Generic;

namespace Squadio.DTO.Models.Notifications
{
    public class EmailMessageDTO
    {
        public string[] ToEmails { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string TemplateId { get; set; }
        public IDictionary<string, string> Args { get; set; }
        public bool Html { get; set; }
    }
}