using System;
using System.Threading.Tasks;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.SignUp
{
    public interface ISignUpRepository
    {
        Task<UserRegistrationStepModel> GetRegistrationStepByEmail(string email);
        Task<UserRegistrationStepModel> GetRegistrationStepByUserId(Guid userId);
        /// <summary>
        /// If entity doesn't exist it will be created, else - updated
        /// </summary>
        Task<UserRegistrationStepModel> SetRegistrationStep(Guid userId, RegistrationStep step);
    }
}