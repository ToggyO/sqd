using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.Domain.Models.Companies;
using Squadio.DTO.Companies;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Companies.Implementation
{
    public class CompaniesProvider : ICompaniesProvider
    {
        private readonly ICompaniesRepository _repository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly IMapper _mapper;
        public CompaniesProvider(ICompaniesRepository repository
            , ICompaniesUsersRepository companiesUsersRepository
            , IMapper mapper)
        {
            _repository = repository;
            _companiesUsersRepository = companiesUsersRepository;
            _mapper = mapper;
        }

        public async Task<Response<PageModel<CompanyDTO>>> GetCompanies(PageModel model)
        {
            var page = await _repository.GetCompanies(model);

            var result = new PageModel<CompanyDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = _mapper.Map<IEnumerable<CompanyModel>, IEnumerable<CompanyDTO>>(page.Items)
            };
            return new Response<PageModel<CompanyDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<PageModel<CompanyWithUserRoleDTO>>> GetUserCompanies(Guid userId, PageModel model)
        {
            var page = await _companiesUsersRepository.GetCompaniesUsers(model, userId);
            
            var items = page.Items.Select(x => _mapper.Map<CompanyUserModel, CompanyWithUserRoleDTO>(x)).ToList();

            var result = new PageModel<CompanyWithUserRoleDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = items
            };
            
            return new Response<PageModel<CompanyWithUserRoleDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<PageModel<UserWithRoleDTO>>> GetCompanyUsers(Guid companyId, PageModel model)
        {
            var page = await _companiesUsersRepository.GetCompaniesUsers(model, companyId: companyId);

            var items = page.Items.Select(x => _mapper.Map<CompanyUserModel, UserWithRoleDTO>(x)).ToList();

            var result = new PageModel<UserWithRoleDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = items
            };
            
            return new Response<PageModel<UserWithRoleDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<CompanyDTO>> GetById(Guid id)
        {
            var entity = await _repository.GetById(id);
            var result = _mapper.Map<CompanyModel, CompanyDTO>(entity);
            return new Response<CompanyDTO>
            {
                Data = result
            };
        }
    }
}