using System;

namespace Squadio.DTO.Projects
{
    public class ProjectCreateDTO
    {
        public string Name { get; set; }
        public string[] Emails { get; set; }
        public string ColorHex { get; set; }
    }
}