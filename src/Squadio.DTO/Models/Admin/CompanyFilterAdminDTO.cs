using System;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Users;

namespace Squadio.DTO.Models.Admin
{
    public class CompanyFilterAdminDTO
    {
        public string Search { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}