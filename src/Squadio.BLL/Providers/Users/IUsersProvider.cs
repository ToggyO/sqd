using Squadio.Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Users
{
    public interface IUsersProvider
    {
        Task<Response<IEnumerable<UserDTO>>> GetAll();
        Task<Response<UserDTO>> GetById(Guid id);
        Task<Response<UserDTO>> GetByEmail(string email);
    }
}
