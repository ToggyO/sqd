using System;
using System.Threading.Tasks;
using AutoMapper;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Admins.Implementations
{
    public class AdminsProvider : IAdminsProvider
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;

        public AdminsProvider(IUsersRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        public async Task<Response<PageModel<UserDTO>>> GetUsersPage(PageModel model)
        {
            var usersPage = await _repository.GetPage(model);

            throw new NotImplementedException();
        }

        public async Task<Response<UserDetailDTO>> GetUserDetail(Guid userId)
        {
            var userEntity = await _repository.GetById(userId);
            
            if (userEntity == null)
            {
                return new BusinessConflictErrorResponse<UserDetailDTO>(new Error
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorCodes.Common.NotFound,
                    Field = ErrorFields.User.Id
                });
            }
            
            var result = _mapper.Map<UserModel, UserDetailDTO>(userEntity);
            return new Response<UserDetailDTO>
            {
                Data = result
            };
        }

        public async Task<Response<UserDetailDTO>> GetUserDetail(string email)
        {
            var userEntity = await _repository.GetByEmail(email);
            
            if (userEntity == null)
            {
                return new BusinessConflictErrorResponse<UserDetailDTO>(new Error
                {
                    Code = ErrorCodes.Common.NotFound,
                    Message = ErrorCodes.Common.NotFound,
                    Field = ErrorFields.User.Id
                });
            }
            
            var result = _mapper.Map<UserModel, UserDetailDTO>(userEntity);
            return new Response<UserDetailDTO>
            {
                Data = result
            };
        }
    }
}