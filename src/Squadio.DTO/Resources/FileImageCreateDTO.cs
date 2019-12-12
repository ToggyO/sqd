using Microsoft.AspNetCore.Http;

namespace Squadio.DTO.Resources
{
    public class FileImageCreateDTO
    {
        public IFormFile File { get; set; }
    }
}