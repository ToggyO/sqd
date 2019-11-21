using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Squadio.API.Filters;
using Squadio.API.Handlers.Auth;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;

namespace Squadio.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthHandler _handler;

        public AuthController(IAuthHandler handler)
        {
            _handler = handler;
        }
        
        /// <summary>
        /// Create token using email and password
        /// </summary>
        [HttpPost("token")]
        public async Task<Response<AuthInfoDTO>> Authenticate([Required] CredentialsDTO request)
        {
            return await _handler.Authenticate(request);
        }
        
        /// <summary>
        /// Create token using refresh token
        /// </summary>
        [HttpPut("token")]
        public async Task<ActionResult<Response<TokenDTO>>> RefreshToken([Required] RefreshTokenDTO request)
        {
            return await _handler.RefreshToken(request.RefreshToken);
        }
        
        /// <summary>
        /// Create token using google tokenId (if user does not exist - creates it)
        /// </summary>
        [HttpPost("token/google")]
        public async Task<Response<AuthInfoDTO>> GoogleAuthenticate([Required] GoogleTokenDTO request)
        {
            return await _handler.GoogleAuthenticate(request.Token);
        }
    }
}