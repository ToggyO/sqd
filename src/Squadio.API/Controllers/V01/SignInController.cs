using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squadio.API.Handlers.Auth;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Auth;

namespace Squadio.API.Controllers.V01
{
    [ApiController]
    [ApiVersion("0.1")]
    [Route("api/v{version:apiVersion}/sign-in")]
    public class SignInController : ControllerBase
    {
        private readonly IAuthHandler _handler;

        public SignInController(IAuthHandler handler)
        {
            _handler = handler;
        }
        
        /// <summary>
        /// Create token using email and password
        /// </summary>
        [HttpPost]
        public async Task<Response<AuthInfoDTO>> Authenticate([Required] CredentialsDTO request)
        {
            return await _handler.Authenticate(request);
        }
        
        /// <summary>
        /// Create token using refresh token
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<Response<TokenDTO>>> RefreshToken([Required] RefreshTokenDTO request)
        {
            return await _handler.RefreshToken(request.RefreshToken);
        }
    }
}