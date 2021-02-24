using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Admins
{
    public interface IAdminsProvider
    {
        Task<Response<PageModel<UserDTO>>> GetUsersPage(PageModel model);
        Task<Response<UserDetailDTO>> GetUserDetail(Guid userId);
        Task<Response<UserDetailDTO>> GetUserDetail(string email);
    }
}