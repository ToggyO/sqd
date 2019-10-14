using System;

namespace Squadio.Domain.Models.Projects
{
    public class ProjectModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}