using System.Collections.Generic;

namespace Squadio.DTO.Pages
{
    public class PageModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
    
    public class PageModel<T> : PageModel
    {
        public int Total { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}