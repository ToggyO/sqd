﻿using System;

namespace Squadio.Common.Models.Filters
{
    public class CompanyFilterModel
    {
        public string Search { get; set; }
        public DateTime? CreateFrom { get; set; }
        public DateTime? CreateTo { get; set; }
    }
}