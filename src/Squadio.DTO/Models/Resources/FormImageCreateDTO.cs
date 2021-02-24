using Microsoft.AspNetCore.Http;

namespace Squadio.DTO.Models.Resources
{
    public class FormImageCreateDTO
    {
        public IFormFile File { get; set; }
    }
}