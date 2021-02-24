using System.Collections.Generic;

namespace Squadio.Common.Models.Emails
{
    public class MailNotificationModel
    {
        /// <summary>Profile through which emails will be sent</summary>
        public string Profile { get; set; } = "default";

        /// <summary>Subject of letter</summary>
        public string Subject { get; set; }

        /// <summary>Main content of letter</summary>
        public string Body { get; set; }

        /// <summary>Is html body?</summary>
        public bool Html { get; set; }

        /// <summary>Receivers of letter</summary>
        public string[] ToAddresses { get; set; }

        /// <summary>Email of author of letter</summary>
        public string FromEmail { get; set; }

        /// <summary>Name of author of letter</summary>
        public string FromName { get; set; }

        /// <summary>Blind copy receivers</summary>
        public string[] BccAddresses { get; set; }

        /// <summary>Copy receivers</summary>
        public string[] CcAddresses { get; set; }

        /// <summary>Attached to letter files</summary>
        public MailMessageAttachment[] Attachments { get; set; }

        /// <summary>Headers of letter</summary>
        public IDictionary<string, string> Meta { get; set; }

        /// <summary>Name of template markup</summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Arguments for setting values in a template message (in Body)
        /// </summary>
        public IDictionary<string, string> Args { get; set; }
    }

    public class MailMessageAttachment 
    {
        /// <summary>
        /// Url to file (file will be downloaded from url for attaching to email) <para />
        /// If set value Content and Url, Content will be used in service
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// File content <para />
        /// If set value Content and Url, Content will be used in service
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>Name of file in attachments</summary>
        public string Name { get; set; }
    }
}