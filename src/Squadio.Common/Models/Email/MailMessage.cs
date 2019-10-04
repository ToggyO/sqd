namespace Squadio.Common.Models.Email
{
    public class MailMessage
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Html { get; set; }
        public string[] ToAddresses { get; set; }
        public MailMessageAttachment[] Attachments { get; set; }
        public MailMessageAttachment[] Images { get; set; }
    }
}