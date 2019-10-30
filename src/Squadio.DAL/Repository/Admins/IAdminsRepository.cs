using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.Admins
{
    public interface IAdminsRepository
    {
        Task<IEnumerable<CompanyUserModel>> GetCompanyUser(Guid? userId = null, Guid? companyId = null, UserStatus? status = null);
        Task<PageModel<UserModel>> GetUsers(PageModel pageModel, string search, UserWithCompaniesFilter filter);
    }
}