﻿using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Users;

namespace Squadio.DTO.Users
{
    public class UserRegistrationStepMapper : IMapper<UserRegistrationStepModel, UserRegistrationStepDTO>
    {
        private readonly IMapper _mapper;

        public UserRegistrationStepMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public UserRegistrationStepDTO Map(UserRegistrationStepModel item)
        {
            return new UserRegistrationStepDTO
            {
                Step = (int) item.Step,
                StepName = item.StepName
            };
        }
    }
    
    
    public class EnumerableUserRegistrationStepMapper : IMapper<IEnumerable<UserRegistrationStepModel>, IEnumerable<UserRegistrationStepDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableUserRegistrationStepMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<UserRegistrationStepDTO> Map(IEnumerable<UserRegistrationStepModel> items)
        {
            var result = items.Select(x => _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(x));
            return result;
        }
    }
}