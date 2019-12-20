using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Users;

namespace Squadio.DTO.Users
{
    public class UserCompanyModelMapper : IMapper<CompanyUserModel, UserWithRoleDTO>
    {
        private readonly IMapper _mapper;

        public UserCompanyModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public UserWithRoleDTO Map(CompanyUserModel item)
        {
            var result = new UserWithRoleDTO
            {
                UserId = item.UserId,
                Status = (int) item.Status,
                StatusName = item.Status.ToString(),
                CreatedDate = item.CreatedDate
            };
            if (item.User != null)
            {
                result.User = _mapper.Map<UserModel, UserDTO>(item.User);
            }
            return result;
        }
    }
    
    public class EnumerableUserCompanyModelMapper : IMapper<IEnumerable<CompanyUserModel>, IEnumerable<UserWithRoleDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableUserCompanyModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<UserWithRoleDTO> Map(IEnumerable<CompanyUserModel> items)
        {
            var result = items.Select(x => _mapper.Map<CompanyUserModel, UserWithRoleDTO>(x));
            return result;
        }
    }
}