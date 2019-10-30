using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Providers.Users;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Admins;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Companies;
using Squadio.DTO.Pages;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Admins.Implementation
{
    public class AdminsProvider : IAdminsProvider
    {
        private readonly IAdminsRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUsersProvider _usersProvider;

        public AdminsProvider(IAdminsRepository repository
            , IMapper mapper
            , IUsersProvider usersProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _usersProvider = usersProvider;
        }
        
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetPage(PageModel model, string search)
        {
            var usersPage = await _repository.GetUsers(model, search);

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
                var items = await _repository.GetCompanyUser(userId: user.Id);
                resultDataItems.Add(new UserWithCompaniesDTO
                {
                    User = _mapper.Map<UserModel, UserDTO>(user),
                    Companies = items.Select(x => new CompanyOfUserDTO
                    {
                        Id = x.CompanyId,
                        Name = x.Company?.Name,
                        Status = (int) x.Status,
                        StatusName = x.Status.ToString()
                    })
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