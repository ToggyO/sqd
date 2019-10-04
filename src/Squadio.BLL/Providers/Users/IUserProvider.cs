using Squadio.Domain.Models.Users;
using System;
using System.Threading.Tasks;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Users
{
    public interface IUserProvider
    {
        Task<UserDTO> GetById(Guid id);
    }
}
