using System;

namespace Squadio.DTO.Projects
{
    public class CreateProjectDTO
    {
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
        public Guid[] UsersId { get; set; }
    }
}