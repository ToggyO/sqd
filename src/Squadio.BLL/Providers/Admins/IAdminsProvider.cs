using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Providers.Admins
{
    public interface IAdminsProvider
    {
        Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage(PageModel model, UserFilterAdminDTO filter);
        Task<Response> SetUserStatus(Guid userId, UserStatus status);
        Task<Response<UserDetailDTO>> GetUserDetail(Guid userId);
        Task<Response<UserDetailDTO>> GetUserDetail(string email);
    }
}