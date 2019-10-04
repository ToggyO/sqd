using Squadio.Domain.Models.Users;
using System;
using System.Threading.Tasks;

namespace Squadio.API.Handlers.Users
{
    public interface IUserHandler
    {
        Task<UserModel> GetById(Guid id);
    }
}
