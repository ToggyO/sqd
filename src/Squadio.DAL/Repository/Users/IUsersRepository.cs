using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.Users
{
    public interface IUsersRepository : IBaseRepository<UserModel>
    {
        Task<IEnumerable<UserModel>> GetAll();
        Task<UserModel> GetByEmail(string email);
        Task<UserPasswordRequestModel> AddChangePasswordRequest(Guid userId, string code);
        Task<UserPasswordRequestModel> GetChangePasswordRequests(string email, string code);
        Task ActivateChangePasswordRequestsCode(Guid userId, string code);
        Task SavePassword(Guid userId, string hash, string salt);
    }
}
