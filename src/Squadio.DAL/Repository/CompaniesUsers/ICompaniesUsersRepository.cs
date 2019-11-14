using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.CompaniesUsers
{
    public interface ICompaniesUsersRepository
    {
        Task<PageModel<CompanyUserModel>> GetCompanyUsers(Guid companyId, PageModel model);
        Task<PageModel<CompanyUserModel>> GetUserCompanies(Guid userId, PageModel model);
        Task<CompanyUserModel> GetCompanyUser(Guid companyId, Guid userId);
        Task<IEnumerable<CompanyUserModel>> GetCompanyUser(Guid? userId = null, Guid? companyId = null, IEnumerable<UserStatus> statuses = null);
        Task<int> GetCompanyUsersCount(Guid companyId);
        Task AddCompanyUser(Guid companyId, Guid userId, UserStatus userStatus);
        Task DeleteCompanyUser(Guid companyId, Guid userId);
        Task AddRangeCompanyUser(Guid companyId, IEnumerable<Guid> userIds, UserStatus userStatus);
        Task ChangeStatusCompanyUser(Guid companyId, Guid userId, UserStatus newUserStatus);
    }
}