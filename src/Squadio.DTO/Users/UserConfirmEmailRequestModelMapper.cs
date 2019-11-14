using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Users;

namespace Squadio.DTO.Users
{
    public class UserConfirmEmailRequestModelMapper: IMapper<UserConfirmEmailRequestModel, UserConfirmEmailRequestDTO>
    {
        private readonly IMapper _mapper;

        public UserConfirmEmailRequestModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public UserConfirmEmailRequestDTO Map(UserConfirmEmailRequestModel item)
        {
            var result = new UserConfirmEmailRequestDTO
            {
                Id = item.Id,
                IsActivated = item.IsActivated,
                ActivatedDate = item.ActivatedDate,
                Code = item.Code,
                CreatedDate = item.CreatedDate,
                UserId = item.UserId
            };
            
            if (item.User != null)
            {
                result.User = _mapper.Map<UserModel, UserDTO>(item.User);
            }
            
            return result;
        }
    }
    
    public class EnumerableUserConfirmEmailRequestModelMapper : IMapper<IEnumerable<UserConfirmEmailRequestModel>, IEnumerable<UserConfirmEmailRequestDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableUserConfirmEmailRequestModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<UserConfirmEmailRequestDTO> Map(IEnumerable<UserConfirmEmailRequestModel> items)
        {
            var result = items.Select(x => _mapper.Map<UserConfirmEmailRequestModel, UserConfirmEmailRequestDTO>(x));
            return result;
        }
    }
}