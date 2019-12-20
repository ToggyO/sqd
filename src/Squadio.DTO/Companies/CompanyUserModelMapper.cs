
using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.DTO.Companies
{
    public class CompanyUserModelMapper : IMapper<CompanyUserModel, CompanyWithUserRoleDTO>
    {
        private readonly IMapper _mapper;

        public CompanyUserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public CompanyWithUserRoleDTO Map(CompanyUserModel item)
        {
            var result = new CompanyWithUserRoleDTO
            {
                CompanyId = item.CompanyId,
                Status = (int) item.Status,
                StatusName = item.Status.ToString(),
                CreatedDate = item.CreatedDate
            };
            if (item.Company != null)
            {
                result.Company = _mapper.Map<CompanyModel, CompanyDTO>(item.Company);
            }
            return result;
        }
    }
    
    public class EnumerableCompanyUserModelMapper : IMapper<IEnumerable<CompanyUserModel>, IEnumerable<CompanyWithUserRoleDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableCompanyUserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<CompanyWithUserRoleDTO> Map(IEnumerable<CompanyUserModel> items)
        {
            var result = items.Select(x => _mapper.Map<CompanyUserModel, CompanyWithUserRoleDTO>(x));
            return result;
        }
    }
}