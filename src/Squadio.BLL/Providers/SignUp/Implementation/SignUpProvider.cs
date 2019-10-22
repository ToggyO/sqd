using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.SignUp;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.SignUp.Implementation
{
    public class SignUpProvider : ISignUpProvider
    {
        private readonly ISignUpRepository _repository;
        private readonly IMapper _mapper;
        public SignUpProvider(ISignUpRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<UserRegistrationStepDTO>> GetRegistrationStep(string email)
        {
            var entity = await _repository.GetRegistrationStepByEmail(email);
            if (entity == null)
            {
                return new BusinessConflictErrorResponse<UserRegistrationStepDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists,
                        Field = ErrorFields.User.Email
                    }
                });
            }
            
            var result = _mapper.Map<UserRegistrationStepModel, UserRegistrationStepDTO>(entity);
            return new Response<UserRegistrationStepDTO>
            {
                Data = result
            };
        }
    }
}