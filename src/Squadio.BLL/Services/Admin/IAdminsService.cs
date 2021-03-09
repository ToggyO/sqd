using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Services.Admin
{
    public interface IAdminsService
    {
        Task<Response> SetPassword(string email, string password);
        Task<Response> ResetPassword(string code, string password);
        Task<Response> ResetPasswordRequest(string email);
        Task<Response> ChangeEmailRequest(Guid id, string newEmail);
        Task<Response<UserDTO>> ChangeEmailConfirm(Guid id, string code);
        Task<Response> CreateAdmin(string email, string name, string password);
        Task<Response> SetUserStatus(Guid userId, UserStatus status);
        Task<Response> SetCompanyStatus(Guid companyId, CompanyStatus status);
    }
}