﻿using System.IO;

namespace Squadio.DTO.Resources
{
    public class FileCreateDTO
    {
        public string ContentType { get; set; }
        public MemoryStream Stream { get; set; }
    }
}