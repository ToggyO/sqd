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
        Task<PageModel<CompanyUserModel>> GetCompaniesUsers(PageModel model
            , Guid? userId = null
            , Guid? companyId = null
            , IEnumerable<MembershipStatus> statuses = null);
        Task<IEnumerable<CompanyUserModel>> GetCompanyUsersByEmails(Guid companyId, IEnumerable<string> emails);
        Task<CompanyUserModel> GetCompanyUser(Guid companyId, Guid userId);
        Task<CompanyUserModel> AddCompanyUser(Guid companyId, Guid userId, MembershipStatus membershipStatus);
        Task DeleteCompanyUser(Guid companyId, Guid userId);
        Task<IEnumerable<CompanyUserModel>> GetCompaniesByUsers(IEnumerable<Guid> userIds);
        Task<IEnumerable<CompanyUserModel>> GetUsersByCompanies(IEnumerable<Guid> companyIds);
    }
}