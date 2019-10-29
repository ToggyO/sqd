using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.DTO.Pages;

namespace Squadio.DAL.Repository.Admins
{
    public interface IAdminsRepository
    {
        Task<IEnumerable<CompanyUserModel>> GetCompanyUser(Guid? userId = null, Guid? companyId = null, UserStatus? status = null);
    }
}