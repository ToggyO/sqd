using System;
using System.Threading.Tasks;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.ChangeEmail
{
    public interface IChangeEmailRequestRepository
    {
        Task<UserChangeEmailRequestModel> AddRequest(Guid userId, string code, string newEmail);
        Task<UserChangeEmailRequestModel> GetRequest(Guid userId, string code);
        Task<UserChangeEmailRequestModel> GetRequest(string email, string code);
        Task ActivateAllRequestsForUser(string userEmail);
        Task ActivateAllRequestsForUser(Guid userId);
    }
}