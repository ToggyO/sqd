﻿using System;

namespace Squadio.Common.Models.Filters
{
    public class CompanyFilter
    {
        public Guid? UserId { get; set; }
        public Guid? CompanyId { get; set; }
    }
}