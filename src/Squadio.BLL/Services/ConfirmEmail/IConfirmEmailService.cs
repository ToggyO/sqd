using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.ConfirmEmail
{
    public interface IConfirmEmailService
    {
        Task<Response<UserConfirmEmailRequestDTO>> AddRequest(Guid userId, string email);
        Task<Response<UserConfirmEmailRequestDTO>> GetRequest(Guid userId, string code);
        Task<Response> ActivateAllRequests(Guid userId);
    }
}