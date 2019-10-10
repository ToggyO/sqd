using Squadio.Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Users
{
    public interface IUsersProvider
    {
        Task<IEnumerable<UserDTO>> GetAll();
        Task<UserDTO> GetById(Guid id);
        Task<UserDTO> GetByEmail(string email);
    }
}
