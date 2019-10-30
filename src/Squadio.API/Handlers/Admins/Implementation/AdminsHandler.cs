using System.Threading.Tasks;
using Squadio.BLL.Providers.Admins;
using Squadio.BLL.Services.Admins;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Admins.Implementation
{
    public class AdminsHandler : IAdminsHandler
    {
        private readonly IAdminsProvider _provider;
        private readonly IAdminsService _service;

        public AdminsHandler(IAdminsProvider provider
            , IAdminsService service)
        {
            _provider = provider;
            _service = service;
        }
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetPage(PageModel model, string search, UserWithCompaniesFilter filter)
        {
            var result = await _provider.GetPage(model, search, filter);
            return result;
        }
    }
}