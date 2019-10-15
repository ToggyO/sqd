using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.SignUp
{
    public interface ISignUpProvider
    {
        Task<Response<UserRegistrationStepDTO>> GetRegistrationStep(string email);
    }
}