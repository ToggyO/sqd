using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.Users
{
    public interface IUsersRepository : IBaseRepository<UserModel>
    {
        Task<PageModel<UserModel>> GetPage(PageModel model, UserFilterModel filter = null);
        Task<UserModel> GetByEmail(string email);
        Task<UserModel> GetDetail(Guid id);
        Task<UserModel> GetDetail(string email);
        Task SavePassword(Guid userId, string hash, string salt);
    }
}
