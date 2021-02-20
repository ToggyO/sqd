using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users
{
    public interface IUsersHandler
    {
        Task<Response<PageModel<UserDTO>>> GetPage(PageModel model);
        Task<Response<UserDTO>> DeleteUser(Guid id);
        Task<Response<UserDTO>> GetById(Guid id);
        Task<Response<UserDTO>> GetCurrentUser(ClaimsPrincipal claims);
    }
}
