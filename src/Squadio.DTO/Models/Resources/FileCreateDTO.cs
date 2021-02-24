using System.IO;

namespace Squadio.DTO.Models.Resources
{
    public class FileCreateDTO
    {
        public string ContentType { get; set; }
        public MemoryStream Stream { get; set; }
    }
}