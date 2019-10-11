using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.DAL.Repository.Companies;
using Squadio.Domain.Models.Companies;
using Squadio.DTO.Companies;

namespace Squadio.BLL.Providers.Companies.Implementation
{
    public class CompaniesProvider : ICompaniesProvider
    {
        private readonly ICompaniesRepository _repository;
        private readonly IMapper _mapper;
        public CompaniesProvider(ICompaniesRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CompanyDTO> GetById(Guid id)
        {
            var entity = await _repository.GetById(id);
            var result = _mapper.Map<CompanyModel, CompanyDTO>(entity);
            return result;
        }
    }
}