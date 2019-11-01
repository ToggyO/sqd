using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;
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

        public async Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies(Guid userId, PageModel model)
        {
            var page = await _companiesUsersRepository.GetUserCompanies(userId, model);

            var result = new PageModel<CompanyUserDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = page.Items.Select(x=>
                {
                    x.User = null;
                    return _mapper.Map<CompanyUserModel, CompanyUserDTO>(x);
                })
            };
            
            return new Response<PageModel<CompanyUserDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<PageModel<CompanyUserDTO>>> GetCompanyUsers(Guid companyId, PageModel model)
        {
            var page = await _companiesUsersRepository.GetCompanyUsers(companyId, model);

            var result = new PageModel<CompanyUserDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = page.Items.Select(x=>
                {
                    x.Company = null;
                    return _mapper.Map<CompanyUserModel, CompanyUserDTO>(x);
                })
            };
            
            return new Response<PageModel<CompanyUserDTO>>
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