using System;
using Squadio.DTO.Users;

namespace Squadio.DTO.Companies
{
    public class CompanyUserDTO
    {
        public Guid CompanyId { get; set; }
        public CompanyDTO Company { get; set; }
        public Guid UserId { get; set; }
        public UserDTO User { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// When user added to company
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}