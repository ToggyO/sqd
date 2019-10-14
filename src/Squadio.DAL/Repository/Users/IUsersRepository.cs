using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.Users
{
    public interface IUsersRepository : IBaseRepository<UserModel>
    {
        Task<IEnumerable<UserModel>> GetAll();
        Task<UserModel> GetByEmail(string email);
        Task<UserRegistrationStepModel> GetRegistrationStepByEmail(string email);
        Task<UserRegistrationStepModel> GetRegistrationStepByUserId(Guid userId);
        /// <summary>
        /// If step == New then entity will be created, else - updated
        /// </summary>
        Task<UserRegistrationStepModel> SetRegistrationStep(Guid userId, RegistrationStep step);
        Task<UserPasswordRequestModel> AddPasswordRequest(Guid userId, string code);
        Task<UserPasswordRequestModel> GetChangePasswordRequests(string email, string code);
        Task ActivateChangePasswordRequestsCode(string code);
        Task SavePassword(Guid userId, string hash, string salt);
    }
}
