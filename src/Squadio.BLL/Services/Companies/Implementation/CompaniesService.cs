using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.DAL.Repository.Companies;
using Squadio.Domain.Models.Companies;
using Squadio.DTO.Companies;

namespace Squadio.BLL.Services.Companies.Implementation
{
    public class CompaniesService : ICompaniesService
    {
        private readonly ICompaniesRepository _repository;
        private readonly IMapper _mapper;
        public CompaniesService(ICompaniesRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CompanyDTO> Create(Guid userId, CreateCompanyDTO dto)
        {
            var entityCompany = new CompanyModel
            {
                Name = dto.Name
            };
            
            entityCompany = await _repository.Create(entityCompany);
            
            //TODO: Add admin
            
            var result = _mapper.Map<CompanyModel, CompanyDTO>(entityCompany);
            return result;
        }
    }
}