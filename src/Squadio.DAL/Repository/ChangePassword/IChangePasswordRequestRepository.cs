using System;
using System.Threading.Tasks;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.ChangePassword
{
    public interface IChangePasswordRequestRepository
    {
        Task<UserPasswordRequestModel> AddRequest(Guid userId, string code);
        Task<UserPasswordRequestModel> GetRequestByCode(string code);
        Task<UserPasswordRequestModel> GetRequest(Guid id);
        Task ActivateAllRequestsForUser(Guid userId);
    }
}