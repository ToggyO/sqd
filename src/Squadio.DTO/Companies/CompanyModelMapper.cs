using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Companies;

namespace Squadio.DTO.Companies
{
    public class CompanyModelMapper : IMapper<CompanyModel, CompanyDTO>
    {
        private readonly IMapper _mapper;

        public CompanyModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public CompanyDTO Map(CompanyModel item)
        {
            return new CompanyDTO
            {
                Id = item.Id,
                Name = item.Name,
                Address = item.Address,
                CreatedDate = item.CreatedDate
            };
        }
    }
    
    public class EnumerableCompanyModelMapper : IMapper<IEnumerable<CompanyModel>, IEnumerable<CompanyDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableCompanyModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<CompanyDTO> Map(IEnumerable<CompanyModel> items)
        {
            var result = items.Select(x => _mapper.Map<CompanyModel, CompanyDTO>(x));
            return result;
        }
    }
}