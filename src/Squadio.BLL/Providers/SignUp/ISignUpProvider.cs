using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.SignUp;

namespace Squadio.BLL.Providers.SignUp
{
    public interface ISignUpProvider
    {
        Task<Response<SignUpStepDTO>> GetRegistrationStep(Guid userId);
        Task<Response<IEnumerable<string>>> GetTeamInvites(Guid userId);
    }
}