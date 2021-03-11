using System;
using System.Collections.Generic;

namespace Squadio.Common.Models.Pages
{
    public class PageModel
    {
        /// <summary>
        /// Page start with 1. Like in front-end
        /// </summary>
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public static PageModel Max()
        {
            return new PageModel{Page = 1, PageSize = Int32.MaxValue};
        }
    }
    
    public class PageModel<T> : PageModel
    {
        public int Total { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}