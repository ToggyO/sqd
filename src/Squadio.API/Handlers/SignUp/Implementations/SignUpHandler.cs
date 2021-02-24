using System.Threading.Tasks;
using Squadio.BLL.Services.SignUp;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.SignUp;

namespace Squadio.API.Handlers.SignUp.Implementations
{
    public class SignUpHandler : ISignUpHandler
    {
        private readonly ISignUpService _service;
        public SignUpHandler(ISignUpService service)
        {
            _service = service;
        }
        
        public async Task<Response> SignUp(SignUpSimpleDTO dto)
        {
            return await _service.SignUp(dto);
        }
    }
}