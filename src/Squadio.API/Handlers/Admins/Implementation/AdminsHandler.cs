using System.Threading.Tasks;
using Squadio.BLL.Providers.Admins;
using Squadio.BLL.Services.Admins;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Pages;
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
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetPage(PageModel model)
        {
            var result = await _provider.GetPage(model);
            return result;
        }
    }
}