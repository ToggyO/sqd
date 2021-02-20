using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.Admins
{
    public interface IAdminsRepository
    {
        Task<PageModel<UserModel>> GetUsers(PageModel pageModel, string search);
        Task<UserModel> GetUserById(Guid userId);
    }
}