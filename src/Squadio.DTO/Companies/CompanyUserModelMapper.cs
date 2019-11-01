
using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.DTO.Companies
{
    public class CompanyUserModelMapper : IMapper<CompanyUserModel, CompanyUserDTO>
    {
        private readonly IMapper _mapper;

        public CompanyUserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public CompanyUserDTO Map(CompanyUserModel item)
        {
            var result = new CompanyUserDTO
            {
                UserId = item.UserId,
                CompanyId = item.CompanyId,
                Status = (int) item.Status,
                StatusName = item.Status.ToString(),
                CreatedDate = item.CreatedDate
            };
            if (item.User != null)
            {
                result.User = _mapper.Map<UserModel, UserDTO>(item.User);
            }
            if (item.Company != null)
            {
                result.Company = _mapper.Map<CompanyModel, CompanyDTO>(item.Company);
            }
            return result;
        }
    }
    
    public class EnumerableCompanyUserModelMapper : IMapper<IEnumerable<CompanyUserModel>, IEnumerable<CompanyUserDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableCompanyUserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<CompanyUserDTO> Map(IEnumerable<CompanyUserModel> items)
        {
            var result = items.Select(x => _mapper.Map<CompanyUserModel, CompanyUserDTO>(x));
            return result;
        }
    }
}