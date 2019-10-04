using Squadio.Domain.Models.Users;
using System;
using System.Threading.Tasks;

namespace Squadio.BLL.Providers.Users
{
    public interface IUserProvider
    {
        Task<UserModel> GetById(Guid id);
    }
}
