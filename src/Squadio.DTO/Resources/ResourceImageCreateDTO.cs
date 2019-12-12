namespace Squadio.DTO.Resources
{
    public class ResourceImageCreateDTO
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Bytes { get; set; }
    }
}