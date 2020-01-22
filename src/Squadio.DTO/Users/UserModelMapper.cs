using System.Collections.Generic;
using System.Linq;
using Squadio.Domain.Models.Users;
using Mapper;
using Microsoft.Extensions.Options;
using Squadio.Common.Models.Resources;
using Squadio.Common.Settings;
using Squadio.DTO.Resources;

namespace Squadio.DTO.Users
{
    public class UserModelMapper : IMapper<UserModel, UserDTO>
    {
        private readonly IMapper _mapper;
        private readonly IOptions<FileTemplateUrlModel> _options;

        public UserModelMapper(IMapper mapper
            , IOptions<FileTemplateUrlModel> options)
        {
            _mapper = mapper;
            _options = options;
        }
    
        public UserDTO Map(UserModel item)
        {
            var result = new UserDTO
            {
                Id = item.Id,
                Email = item.Email,
                Name = item.Name,
                UITheme = item.UITheme.ToString(),
                SignUpBy = item.SignUpType.ToString()
            };
            if (result.Name == null)
            {
                result.Name = "Username";
            }
            if (item.Avatar != null)
            {
                var viewModel = new ResourceImageViewModel(item.Avatar, _options.Value.ImageTemplate);
                result.Avatar = _mapper.Map<ResourceImageViewModel, ResourceImageDTO>(viewModel);
            }
            return result;
        }
    }
    
    public class EnumerableUserModelMapper : IMapper<IEnumerable<UserModel>, IEnumerable<UserDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableUserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<UserDTO> Map(IEnumerable<UserModel> items)
        {
            var result = items.Select(x => _mapper.Map<UserModel, UserDTO>(x));
            return result;
        }
    }
}