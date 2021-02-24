namespace Squadio.DTO.Models.Resources
{
    public class ByteImageCreateDTO
    {
        public string ContentType { get; set; }
        public byte[] Bytes { get; set; }
    }
}