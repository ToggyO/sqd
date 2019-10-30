using System;

namespace Squadio.DTO.Companies
{
    public class CompanyDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}