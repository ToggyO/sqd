using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.SignUp;
using Squadio.Common.Models.Responses;
using Squadio.DTO.SignUp;

namespace Squadio.API.Controllers.V01
{
    [ApiController]
    [ApiVersion("0.1")]
    [Route("api/v{version:apiVersion}/sign-up")]
    public class SignUpController : ControllerBase
    {
        private readonly ISignUpHandler _handler;

        public SignUpController(ISignUpHandler handler)
        {
            _handler = handler;
        }
        
        /// <summary>
        /// Very simple creating user
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<Response> SignUp([Required, FromBody] SignUpSimpleDTO dto)
        {
            return await _handler.SignUp(dto);
        }
    }
}