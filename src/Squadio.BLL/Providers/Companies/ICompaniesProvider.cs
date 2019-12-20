using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Companies
{
    public interface ICompaniesProvider
    {
        Task<Response<PageModel<CompanyDTO>>> GetCompanies(PageModel model);
        Task<Response<PageModel<CompanyWithUserRoleDTO>>> GetUserCompanies(Guid userId, PageModel model);
        Task<Response<PageModel<UserWithRoleDTO>>> GetCompanyUsers(Guid companyId, PageModel model);
        Task<Response<CompanyDTO>> GetById(Guid id);
    }
}