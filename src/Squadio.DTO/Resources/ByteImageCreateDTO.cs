using System.IO;

namespace Squadio.DTO.Resources
{
    public class ByteImageCreateDTO
    {
        public string ContentType { get; set; }
        public byte[] Bytes { get; set; }
    }
}