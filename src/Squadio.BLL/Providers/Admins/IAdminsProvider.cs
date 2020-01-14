using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Common.Models.Sorts;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Admins
{
    public interface IAdminsProvider
    {
        Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage(PageModel model, string search, UserWithCompaniesFilter filter);
        Task<Response<PageModel<CompanyListDTO>>> GetCompaniesPage(PageModel model
            , CompanyAdminFilter filter
            , SortCompaniesModel sort
            , string search);
        Task<Response<CompanyDetailDTO>> GetCompanyDetail(Guid companyId);
        Task<Response<UserDTO>> GetUserDetail(Guid userId);
    }
}