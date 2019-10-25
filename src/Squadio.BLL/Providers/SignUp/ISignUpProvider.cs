using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.SignUp
{
    public interface ISignUpProvider
    {
        Task<Response<SignUpStepDTO>> GetRegistrationStep(string email);
        Task<Response<IEnumerable<string>>> GetTeamInvites(Guid userId);
    }
}