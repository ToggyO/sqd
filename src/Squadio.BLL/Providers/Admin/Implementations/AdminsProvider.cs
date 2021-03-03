using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Models.Admin;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Providers.Admin.Implementations
{
    public class AdminsProvider : IAdminsProvider
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ICompaniesRepository _companiesRepository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly IMapper _mapper;

        public AdminsProvider(IUsersRepository usersRepository
            , ICompaniesRepository companiesRepository
            , ICompaniesUsersRepository companiesUsersRepository
            , IMapper mapper)
        {
            _usersRepository = usersRepository;
            _companiesRepository = companiesRepository;
            _companiesUsersRepository = companiesUsersRepository;
            _mapper = mapper;
        }
        
        public async Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage(PageModel model, UserFilterAdminDTO filter)
        {
            var mappedFilter = _mapper.Map<UserFilterModel>(filter);
            var usersEntityPage = await _usersRepository.GetPage(model, mappedFilter);
            
            var usersDtoPage = _mapper.Map<PageModel<UserWithCompaniesDTO>>(usersEntityPage);

            var usersCompaniesEntityList = (await _companiesUsersRepository.GetCompaniesByUsers(
                usersDtoPage.Items.Select(x => x.User.Id))).ToList();

            foreach (var user in usersDtoPage.Items)
            {
                user.Companies = _mapper.Map<IEnumerable<CompanyWithUserRoleDTO>>(
                    usersCompaniesEntityList.Where(x => x.UserId == user.User.Id));
            }

            return new Response<PageModel<UserWithCompaniesDTO>>
            {
                Data = usersDtoPage
            };
        }

        public async Task<Response> SetUserStatus(Guid userId, UserStatus status)
        {
            var userEntity = await _usersRepository.GetById(userId);
            
            if (userEntity == null)
            {
                return new NotFoundErrorResponse();
            }
            
            if (userEntity.RoleId == RoleGuid.Admin)
            {
                return new ForbiddenErrorResponse();
            }
            
            if(userEntity.Status == status)
                return new Response();

            userEntity.Status = status;
            await _usersRepository.Update(userEntity);
            return new Response();
        }

        public async Task<Response<UserDetailAdminDTO>> GetUserDetail(Guid userId)
        {
            var userEntity = await _usersRepository.GetDetail(userId);
            
            if (userEntity == null)
            {
                return new NotFoundErrorResponse<UserDetailAdminDTO>();
            }
            
            var result = _mapper.Map<UserModel, UserDetailAdminDTO>(userEntity);
            return new Response<UserDetailAdminDTO>
            {
                Data = result
            };
        }

        public async Task<Response<UserDetailAdminDTO>> GetUserDetail(string email)
        {
            var userEntity = await _usersRepository.GetDetail(email);
            
            if (userEntity == null)
            {
                return new NotFoundErrorResponse<UserDetailAdminDTO>();
            }
            
            var result = _mapper.Map<UserModel, UserDetailAdminDTO>(userEntity);
            return new Response<UserDetailAdminDTO>
            {
                Data = result
            };
        }

        public async Task<Response<PageModel<CompanyDetailAdminDTO>>> GetCompanyPage(PageModel model, CompanyFilterAdminDTO filter)
        {
            var mappedFilter = _mapper.Map<CompanyFilterModel>(filter);
            var companiesEntityPage = await _companiesRepository.GetCompanies(model, mappedFilter);
            
            var companyDtoPage = _mapper.Map<PageModel<CompanyDetailAdminDTO>>(companiesEntityPage);
            
            var usersCompaniesEntityList = (await _companiesUsersRepository.GetUsersByCompanies(
                companyDtoPage.Items.Select(x => x.Id))).ToList();

            foreach (var company in companyDtoPage.Items)
            {
                var companyUsers = usersCompaniesEntityList.Where(x => x.CompanyId == company.Id).ToList();
                company.UsersCount = companyUsers.Count(x => x.User.Status != UserStatus.Active);
                company.Admins = _mapper.Map<IEnumerable<UserWithRoleDTO>>(companyUsers.Where(x => x.Status != MembershipStatus.Member));
            }

            return new Response<PageModel<CompanyDetailAdminDTO>>
            {
                Data = companyDtoPage
            };
        }

        public async Task<Response<CompanyDetailAdminDTO>> GetCompanyDetail(Guid companyId)
        {
            throw new NotImplementedException();
        }
    }
}