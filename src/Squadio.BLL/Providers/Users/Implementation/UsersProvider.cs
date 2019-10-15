using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Users.Implementation
{
    public class UsersProvider : IUsersProvider
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;
        public UsersProvider(IUsersRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<IEnumerable<UserDTO>>> GetAll()
        {
            var userEntities = await _repository.GetAll();
            var result = _mapper.Map<IEnumerable<UserModel>, IEnumerable<UserDTO>>(userEntities);
            return new Response<IEnumerable<UserDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<UserDTO>> GetById(Guid id)
        {
            var userEntity = await _repository.GetById(id);
            var result = _mapper.Map<UserModel, UserDTO>(userEntity);
            return new Response<UserDTO>
            {
                Data = result
            };
        }

        public async Task<Response<UserDTO>> GetByEmail(string email)
        {
            var userEntity = await _repository.GetByEmail(email);
            var result = _mapper.Map<UserModel, UserDTO>(userEntity);
            return new Response<UserDTO>
            {
                Data = result
            };
        }
    }
}
