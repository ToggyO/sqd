using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Exceptions.SecurityExceptions;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.SignUp.Implementation
{
    public class SignUpProvider : ISignUpProvider
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;
        public SignUpProvider(IUsersRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserRegistrationStepDTO> GetRegistrationStep(string email)
        {
            var entity = await _repository.GetRegistrationStepByEmail(email);
            if(entity == null) 
                throw new SecurityException("","User not registered");
            
            var result = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(entity);
            return result;
        }
    }
}