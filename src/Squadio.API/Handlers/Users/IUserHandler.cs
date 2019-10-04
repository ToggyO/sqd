using Squadio.Domain.Models.Users;
using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users
{
    public interface IUserHandler
    {
        Task<Response<UserDTO>> GetById(Guid id);
        Task<Response> SignUp(string email);
    }
}
