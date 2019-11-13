using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.Users
{
    public interface IUsersRepository : IBaseRepository<UserModel>
    {
        Task<PageModel<UserModel>> GetPage(PageModel model);
        Task<UserModel> GetByEmail(string email);
        Task SavePassword(Guid userId, string hash, string salt);
    }
}
