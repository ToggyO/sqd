using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.SignUp;

namespace Squadio.API.Handlers.SignUp
{
    public interface ISignUpHandler
    {
        Task<Response> SignUp(SignUpSimpleDTO dto);
    }
}