using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Admins
{
    public interface IAdminsProvider
    {
        Task<Response<PageModel<UserWithCompaniesDTO>>> GetPage(PageModel model, string search, UserWithCompaniesFilter filter);
    }
}