using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Companies;
using Squadio.DTO.Pages;
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

        public async Task<Response<IEnumerable<CompanyOfUserDTO>>> GetCompaniesOfUser(Guid? userId, UserStatus? status = null)
        {
            var items = await _repository.Get(userId, null, status);

            var resultItems = items.Select(x => new CompanyOfUserDTO
            {
                Id = x.CompanyId,
                Name = x.Company?.Name,
                Status = (int) x.Status,
                StatusName = x.Status.ToString()
            });
            
            return new Response<IEnumerable<CompanyOfUserDTO>>
            {
                Data = resultItems
            };
        }

        public async Task<Response<PageModel<UserDTO>>> GetCompanyUsers(Guid companyId, PageModel model)
        {
            var page = await _companiesUsersRepository.GetCompanyUsers(companyId, model);

            var result = new PageModel<UserDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = _mapper.Map<IEnumerable<UserModel>,IEnumerable<UserDTO>>(page.Items)
            };
            
            return new Response<PageModel<UserDTO>>
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