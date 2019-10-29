using System;

namespace Squadio.DTO.Companies
{
    public class CompanyOfUserDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }
}