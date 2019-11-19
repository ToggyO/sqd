using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Users
{
    public interface IUsersService
    {
        Task<Response<UserDTO>> SetPassword(string email, string password);
        Task<Response<UserDTO>> ResetPassword(string code, string password);
        Task<Response> ResetPasswordRequest(string email);
        Task<Response<UserDTO>> CreateUser(UserCreateDTO dto);
        Task<Response<UserDTO>> UpdateUser(Guid id, UserUpdateDTO dto);
        Task<Response<UserDTO>> DeleteUser(Guid id);
        Task<Response> ChangeEmailRequest(Guid id, ChangeEmailRequestDTO requestDTO);
        Task<Response<UserDTO>> SetEmail(Guid id, string code);
    }
}
