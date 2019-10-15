using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Companies;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.DTO.Companies;

namespace Squadio.BLL.Services.Companies.Implementation
{
    public class CompaniesService : ICompaniesService
    {
        private readonly ICompaniesRepository _repository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly IMapper _mapper;
        public CompaniesService(ICompaniesRepository repository
            , ICompaniesUsersRepository companiesUsersRepository
            , IMapper mapper)
        {
            _repository = repository;
            _companiesUsersRepository = companiesUsersRepository;
            _mapper = mapper;
        }

        public async Task<Response<CompanyDTO>> Create(Guid userId, CreateCompanyDTO dto)
        {
            var entityCompany = new CompanyModel
            {
                Name = dto.Name,
                CreatedDate = DateTime.UtcNow
            };
            
            entityCompany = await _repository.Create(entityCompany);

            await _companiesUsersRepository.AddCompanyUser(entityCompany.Id, userId, UserStatus.SuperAdmin);
            
            var result = _mapper.Map<CompanyModel, CompanyDTO>(entityCompany);
            
            return new Response<CompanyDTO>
            {
                Data = result
            };
        }
    }
}