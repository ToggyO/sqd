using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Admins
{
    public interface IAdminsProvider
    {
        Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage(PageModel model, string search, UserWithCompaniesFilter filter);

        Task<Response<PageModel<CompanyListDTO>>> GetCompaniesPage(PageModel model, CompaniesFilter filter,
            string search);
    }
}