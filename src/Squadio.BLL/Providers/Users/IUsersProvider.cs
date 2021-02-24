using Squadio.Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Providers.Users
{
    public interface IUsersProvider
    {
        Task<Response<PageModel<UserDTO>>> GetPage(PageModel model);
        Task<Response<UserDTO>> GetById(Guid id);
        Task<Response<UserDTO>> GetByEmail(string email);
        Task<Response> ValidateCode(string code);
    }
}
