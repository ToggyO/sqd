using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Admins;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Admins.Implementation
{
    public class AdminsProvider : IAdminsProvider
    {
        private readonly IAdminsRepository _repository;
        private readonly IMapper _mapper;

        public AdminsProvider(IAdminsRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage(PageModel model, string search, UserWithCompaniesFilter filter)
        {
            var usersPage = await _repository.GetUsers(model, search, filter);

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

        public async Task<Response<PageModel<CompanyListDTO>>> GetCompaniesPage(PageModel model, CompaniesFilter filter, string search)
        {
            var companiesPage = await _repository.GetCompanies(model, filter, search);

            var resultData = new PageModel<CompanyListDTO>()
            {
                Page = companiesPage.Page,
                PageSize = companiesPage.PageSize,
                Total = companiesPage.Total
            };

            var companies = companiesPage.Items;

            var resultDataItems = new List<CompanyListDTO>();
            
            foreach (var company in companies)
            {
                var admins = (await _repository.GetCompanyUser(
                        companyId: company.Id,
                        statuses: new[]
                        {
                            UserStatus.SuperAdmin, 
                            UserStatus.Admin
                        }))
                    .Select(x => x.User);
                resultDataItems.Add(new CompanyListDTO
                {
                    Company = _mapper.Map<CompanyModel, CompanyDTO>(company),
                    UsersCount = await _repository.GetCompanyUsersCount(company.Id),
                    Admins = _mapper.Map<IEnumerable<UserModel>,IEnumerable<UserDTO>>(admins)
                });
            }

            resultData.Items = resultDataItems;
            
            var result = new Response<PageModel<CompanyListDTO>>
            {
                Data = resultData
            };
            return result;
        }
    }
}