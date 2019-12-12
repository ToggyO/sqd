using Microsoft.AspNetCore.Http;

namespace Squadio.DTO.Resources
{
    public class FileCreateDTO
    {
        public IFormFile File { get; set; }
    }
}