using Squadio.Domain.Models.Users;
using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users
{
    public interface IUserHandler
    {
        Task<Response<UserDTO>> GetById(Guid id);
        Task<Response<UserDTO>> GetByCode(string code);
        Task<Response<AuthInfoDTO>> SetPassword(UserSetPasswordDTO dto);
        Task<Response> SignUp(string email);
    }
}
