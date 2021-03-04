﻿using System;

namespace Squadio.Common.Models.Filters
{
    public class CompanyFilterModel
    {
        public string Search { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}