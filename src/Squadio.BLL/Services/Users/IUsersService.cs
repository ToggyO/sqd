
using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Users
{
    public interface IUsersService
    {
        Task SignUp(string email);
        Task<UserDTO> SetPassword(string email, string code, string password);
        Task ResetPasswordRequest(string email);
        Task<UserDTO> UpdateUser(Guid id, UserUpdateDTO updateDTO);
    }
}
