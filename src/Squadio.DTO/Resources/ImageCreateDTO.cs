using System.IO;

namespace Squadio.DTO.Resources
{
    public class ImageCreateDTO
    {
        public string ContentType { get; set; }
        public MemoryStream Stream { get; set; }
    }
}