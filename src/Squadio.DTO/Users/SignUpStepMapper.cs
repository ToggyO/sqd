using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Users;

namespace Squadio.DTO.Users
{
    public class SignUpStepStepMapper : IMapper<UserRegistrationStepModel, SignUpStepDTO>
    {
        private readonly IMapper _mapper;

        public SignUpStepStepMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public SignUpStepDTO Map(UserRegistrationStepModel item)
        {
            return new SignUpStepDTO
            {
                RegistrationStep = new UserRegistrationStepDTO()
                {
                    Step = (int) item.Step,
                    StepName = item.Step.ToString()
                }
            };
        }
    }
    
    
    public class EnumerableSignUpStepStepMapper : IMapper<IEnumerable<UserRegistrationStepModel>, IEnumerable<SignUpStepDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableSignUpStepStepMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<SignUpStepDTO> Map(IEnumerable<UserRegistrationStepModel> items)
        {
            var result = items.Select(x => _mapper.Map<UserRegistrationStepModel, SignUpStepDTO>(x));
            return result;
        }
    }
}