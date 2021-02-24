namespace Squadio.DTO.Models.Resources
{
    public class ByteFileCreateDTO
    {
        public string ContentType { get; set; }
        public byte[] Bytes { get; set; }
    }
}