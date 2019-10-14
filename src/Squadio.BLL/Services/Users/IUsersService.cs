using System;
using System.Threading.Tasks;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Users
{
    public interface IUsersService
    {
        Task<UserDTO> SetPassword(string email, string code, string password);
        Task ResetPasswordRequest(string email);
        Task<UserDTO> UpdateUser(Guid id, UserUpdateDTO updateDTO);
        /// <summary>
        /// Generate number code
        /// </summary>
        /// <param name="length">Count of numbers</param>
        string GenerateCode(int length = 6);
    }
}
