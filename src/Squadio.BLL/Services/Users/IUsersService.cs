using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.BLL.Services.Users
{
    public interface IUsersService
    {
        Task<Response<UserDTO>> SetPassword(string email, string password);
        Task<Response<UserDTO>> ResetPassword(string code, string password);
        Task<Response> ResetPasswordRequest(string email);
        Task<Response<UserDTO>> CreateUser(UserCreateDTO dto);
        Task<Response<UserDTO>> CreateUserWithPasswordRestore(UserCreateDTO dto, string code);
        Task<Response<UserDTO>> UpdateUser(Guid id, UserUpdateDTO dto);
        Task<Response<UserDTO>> DeleteUser(Guid id);
        Task<Response> ChangeEmailRequest(Guid id, string newEmail);
        Task<Response<UserDTO>> SetEmail(Guid id, string code);
        Task<Response<UserDTO>> SaveNewAvatar(Guid userId, Guid resourceId);
        Task<Response<UserDTO>> DeleteAvatar(Guid userId);
    }
}
