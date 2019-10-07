using System.IO;

namespace Squadio.Common.Models.Email
{
    public class EmailAttachment
    {
        public EmailAttachment(string path, string contentMediaType) : this(path, "", contentMediaType)
        {

        }

        public EmailAttachment(string path, bool isInline) : this(path)
        {
            IsInline = isInline;
        }

        public EmailAttachment(string path, string fileName = "", string contentMediaType = "")
        {
            FilePath = path;
            if (string.IsNullOrEmpty(contentMediaType)) ContentMediaType = "image/png";
            if (string.IsNullOrEmpty(fileName)) FileName = Path.GetFileNameWithoutExtension(path) + Path.GetExtension(path);
            IsInline = true;
        }
        
        public bool IsInline { get; set; }

        public string FilePath { get; set; }
        
        public string FileName { get; set; }
        
        public string ContentMediaType { get; set; }
    }
}