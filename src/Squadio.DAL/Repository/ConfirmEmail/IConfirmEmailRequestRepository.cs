using System;
using System.Threading.Tasks;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.ConfirmEmail
{
    public interface IConfirmEmailRequestRepository
    {
        Task<UserConfirmEmailRequestModel> AddRequest(Guid userId, string code);
        Task<UserConfirmEmailRequestModel> GetRequest(Guid userId, string code);
        Task<UserConfirmEmailRequestModel> GetRequest(string email, string code);
        Task ActivateAllRequestsForUser(string userEmail);
        Task ActivateAllRequestsForUser(Guid userId);
    }
}