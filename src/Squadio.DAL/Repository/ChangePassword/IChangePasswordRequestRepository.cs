using System;
using System.Threading.Tasks;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.ChangePassword
{
    public interface IChangePasswordRequestRepository
    {
        Task<UserRestorePasswordRequestModel> AddRequest(Guid userId, string code);
        Task<UserRestorePasswordRequestModel> GetRequestByCode(string code);
        Task<UserRestorePasswordRequestModel> GetRequest(Guid id);
        Task ActivateAllRequestsForUser(Guid userId);
    }
}