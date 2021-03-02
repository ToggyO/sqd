using System;

namespace Squadio.DTO.Models.Companies
{
    public class CompanyWithUserRoleDTO
    {
        public Guid CompanyId { get; set; }
        public CompanyDTO Company { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// When user added to company
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}