using Microsoft.AspNetCore.Http;

namespace Squadio.DTO.Resources
{
    public class FormImageCreateDTO
    {
        public IFormFile File { get; set; }
    }
}