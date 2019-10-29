using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Providers.Users;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Pages;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Admins.Implementation
{
    public class AdminsProvider : IAdminsProvider
    {
        private readonly IUsersProvider _usersProvider;
        private readonly ICompaniesProvider _companiesProvider;

        public AdminsProvider(IUsersProvider usersProvider
            , ICompaniesProvider companiesProvider)
        {
            _usersProvider = usersProvider;
            _companiesProvider = companiesProvider;
        }
        
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetPage(PageModel model)
        {
            var usersResponse = await _usersProvider.GetPage(model);
            if (!usersResponse.IsSuccess)
            {
                var error = (ErrorResponse<PageModel<UserDTO>>) usersResponse;
                return new ErrorResponse<PageModel<UserWithCompaniesDTO>>
                {
                    Code = error.Code,
                    Message = error.Message,
                    HttpStatusCode = error.HttpStatusCode,
                    Errors = error.Errors
                };
            }

            var usersPage = usersResponse.Data;

            var resultData = new PageModel<UserWithCompaniesDTO>()
            {
                Page = usersPage.Page,
                PageSize = usersPage.PageSize,
                Total = usersPage.Total
            };

            var users = usersPage.Items;

            var resultDataItems = new List<UserWithCompaniesDTO>();
            
            foreach (var user in users)
            {
                var companiesResponse = await _companiesProvider.GetCompaniesOfUser(user.Id);
                resultDataItems.Add(new UserWithCompaniesDTO
                {
                    User = user,
                    Companies = companiesResponse.Data
                });
            }

            resultData.Items = resultDataItems;
            
            var result = new Response<PageModel<UserWithCompaniesDTO>>
            {
                Data = resultData
            };

            return result;
        }
    }
}