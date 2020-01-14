using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Common.Models.Sorts;
using Squadio.DAL.Repository.Admins;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
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
        private readonly ICompaniesRepository _companiesRepository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly IMapper _mapper;

        public AdminsProvider(IAdminsRepository repository
            , ICompaniesRepository companiesRepository
            , ICompaniesUsersRepository companiesUsersRepository
            , IMapper mapper)
        {
            _repository = repository;
            _companiesRepository = companiesRepository;
            _companiesUsersRepository = companiesUsersRepository;
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
                var page = await _companiesUsersRepository.GetCompaniesUsers(new PageModel{ PageSize = 1000 }, user.Id);
                var items = page.Items;
                resultDataItems.Add(new UserWithCompaniesDTO
                {
                    User = _mapper.Map<UserModel, UserDTO>(user),
                    Companies = items.Select(x => _mapper.Map<CompanyUserModel, CompanyWithUserRoleDTO>(x))
                });
            }

            resultData.Items = resultDataItems;
            
            var result = new Response<PageModel<UserWithCompaniesDTO>>
            {
                Data = resultData
            };

            return result;
        }

        public async Task<Response<PageModel<CompanyListDTO>>> GetCompaniesPage(PageModel model
            , CompanyAdminFilter filter
            , SortCompaniesModel sort
            , string search)
        {
            var companiesPage = await _companiesRepository.GetCompanies(model, filter, sort, search);

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
                var page = await _companiesUsersRepository.GetCompaniesUsers(new PageModel{ PageSize = 1000 },
                    companyId: company.Id,
                    statuses: new[]
                    {
                        UserStatus.SuperAdmin, 
                        UserStatus.Admin
                    });
                var admins = page.Items;
                resultDataItems.Add(new CompanyListDTO
                {
                    Company = _mapper.Map<CompanyModel, CompanyDTO>(company),
                    UsersCount = await _companiesUsersRepository.GetCompanyUsersCount(company.Id),
                    Admins = admins.Select(x => _mapper.Map<CompanyUserModel, UserWithRoleDTO>(x))
                });
            }

            resultData.Items = resultDataItems;
            
            var result = new Response<PageModel<CompanyListDTO>>
            {
                Data = resultData
            };
            return result;
        }

        public async Task<Response<CompanyDetailDTO>> GetCompanyDetail(Guid companyId)
        {
            var companyEntity = await _companiesRepository.GetById(companyId);
            if (companyEntity == null)
            {
                return new BusinessConflictErrorResponse<CompanyDetailDTO>(new Error
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorMessages.Common.NotFound,
                    Field = ErrorFields.Company.Id
                });
            }

            var companyDetailDTO = _mapper.Map<CompanyModel, CompanyDetailDTO>(companyEntity);
            
            var page = await _companiesUsersRepository.GetCompaniesUsers(new PageModel{ PageSize = 1000 },
                companyId: companyId,
                statuses: new[]
                {
                    UserStatus.SuperAdmin,
                    UserStatus.Admin
                });
            var admins = page.Items;
            companyDetailDTO.UsersCount = await _companiesUsersRepository.GetCompanyUsersCount(companyId);
            companyDetailDTO.Admins = admins.Select(x => _mapper.Map<CompanyUserModel, UserWithRoleDTO>(x));


            return new Response<CompanyDetailDTO>
            {
                Data = companyDetailDTO
            };
        }

        public async Task<Response<UserDTO>> GetUserDetail(Guid userId)
        {
            var userEntity = await _repository.GetUserById(userId);
            
            if (userEntity == null)
            {
                return new BusinessConflictErrorResponse<UserDTO>(new Error
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorCodes.Common.NotFound,
                    Field = ErrorFields.User.Id
                });
            }
            
            var result = _mapper.Map<UserModel, UserDTO>(userEntity);
            return new Response<UserDTO>
            {
                Data = result
            };
        }
    }
}