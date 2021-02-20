using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Admins;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Admins.Implementation
{
    public class AdminsProvider : IAdminsProvider
    {
        private readonly IAdminsRepository _repository;
        private readonly IMapper _mapper;

        public AdminsProvider(IAdminsRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        public async Task<Response<PageModel<UserDTO>>> GetUsersPage(PageModel model, string search)
        {
            var usersPage = await _repository.GetUsers(model, search);

            throw new NotImplementedException();
        }

        public async Task<Response<UserDTO>> GetUserDetail(Guid userId)
        {
            var userEntity = await _repository.GetUserById(userId);
            
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
    }
}