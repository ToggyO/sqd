﻿using System.Collections.Generic;
using System.Linq;
using Squadio.Domain.Models.Users;
using Mapper;

namespace Squadio.DTO.Users
{
    public class UserModelMapper : IMapper<UserModel, UserDTO>
    {
        private readonly IMapper _mapper;

        public UserModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public UserDTO Map(UserModel item)
        {
            return new UserDTO
            {
                Id = item.Id,
                Email = item.Email,
                FirstName = item.FirstName,
                LastName = item.LastName,
                MiddleName = item.MiddleName,
                PhoneNumber = item.PhoneNumber
            };
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