using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Pages;

namespace Squadio.DAL.Repository.Users
{
    public interface IUsersRepository : IBaseRepository<UserModel>
    {
        Task<PageModel<UserModel>> GetPage(PageModel model);
        Task<UserModel> GetByEmail(string email);
        Task<UserPasswordRequestModel> AddChangePasswordRequest(Guid userId, string code);
        Task<UserPasswordRequestModel> GetChangePasswordRequests(string email, string code);
        Task ActivateChangePasswordRequestsCode(Guid userId);
        Task SavePassword(Guid userId, string hash, string salt);
    }
}
