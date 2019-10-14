using System.Threading.Tasks;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.SignUp
{
    public interface ISignUpProvider
    {
        Task<UserRegistrationStepDTO> GetRegistrationStep(string email);
    }
}