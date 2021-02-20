using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Admins
{
    public interface IAdminsProvider
    {
        Task<Response<PageModel<UserDTO>>> GetUsersPage(PageModel model, string search);
        Task<Response<UserDTO>> GetUserDetail(Guid userId);
    }
}