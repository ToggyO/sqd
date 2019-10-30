using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Admins
{
    public interface IAdminsHandler
    {
        Task<Response<PageModel<UserWithCompaniesDTO>>> GetPage(PageModel model, string search, UserWithCompaniesFilter filter);
    }
}