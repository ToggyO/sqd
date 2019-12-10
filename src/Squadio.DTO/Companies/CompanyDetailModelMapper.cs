using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.DTO.Companies
{
    public class CompanyDetailModelMapper: IMapper<CompanyModel, CompanyDetailDTO>
    {
        private readonly IMapper _mapper;

        public CompanyDetailModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public CompanyDetailDTO Map(CompanyModel item)
        {
            var result = new CompanyDetailDTO
            {
                Id = item.Id,
                Name = item.Name,
                Address = item.Address,
                CreatedDate = item.CreatedDate
            };
            
            if (item.Creator != null)
            {
                result.Creator = _mapper.Map<UserModel, UserDTO>(item.Creator);
            }

            return result;
        }
    }
    
    public class EnumerableCompanyDetailModelMapper : IMapper<IEnumerable<CompanyModel>, IEnumerable<CompanyDetailDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableCompanyDetailModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<CompanyDetailDTO> Map(IEnumerable<CompanyModel> items)
        {
            var result = items.Select(x => _mapper.Map<CompanyModel, CompanyDetailDTO>(x));
            return result;
        }
    }
}