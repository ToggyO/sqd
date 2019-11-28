using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Users.Implementation
{
    public class UsersProvider : IUsersProvider
    {
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;
        public UsersProvider(IChangePasswordRequestRepository changePasswordRepository
            , IUsersRepository repository
            , IMapper mapper)
        {
            _changePasswordRepository = changePasswordRepository;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<PageModel<UserDTO>>> GetPage(PageModel model)
        {
            
            var page = await _repository.GetPage(model);

            var result = new PageModel<UserDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = _mapper.Map<IEnumerable<UserModel>,IEnumerable<UserDTO>>(page.Items)
            };
            
            return new Response<PageModel<UserDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<UserDTO>> GetById(Guid id)
        {
            var userEntity = await _repository.GetById(id);
            
            if (userEntity == null)
            {
                return new BusinessConflictErrorResponse<UserDTO>(new Error
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorCodes.Common.NotFound,
                    Field = ErrorFields.User.Id
                });
            }
            
            var result = _mapper.Map<UserModel, UserDTO>(userEntity);
            return new Response<UserDTO>
            {
                Data = result
            };
        }

        public async Task<Response<UserDTO>> GetByEmail(string email)
        {
            var userEntity = await _repository.GetByEmail(email);
            
            if (userEntity == null)
            {
                return new BusinessConflictErrorResponse<UserDTO>(new Error
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorCodes.Common.NotFound,
                    Field = ErrorFields.User.Id
                });
            }
            
            var result = _mapper.Map<UserModel, UserDTO>(userEntity);
            return new Response<UserDTO>
            {
                Data = result
            };
        }

        public async Task<Response> ValidateCode(string code)
        {
            var userPasswordRequest = await _changePasswordRepository.GetRequestByCode(code);
            
            // TODO: Check lifetime of request if needed
            
            if (userPasswordRequest == null 
                || userPasswordRequest?.IsActivated == true)
            {
                return new BusinessConflictErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.PasswordChangeRequestInvalid,
                        Message = ErrorMessages.Business.PasswordChangeRequestInvalid
                    }
                });
            }
            return new Response();
        }
    }
}
