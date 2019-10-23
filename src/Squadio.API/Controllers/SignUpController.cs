using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Squadio.API.Handlers.SignUp;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.SignUp;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Controllers
{
    [ApiController]
    [Route("api/signup")]
    public class SignUpController : ControllerBase
    {
        private readonly ISignUpHandler _handler;

        public SignUpController(ISignUpHandler handler)
        {
            _handler = handler;
        }
        
        [HttpGet("step")]
        public async Task<Response<UserRegistrationStepDTO>> GetStatus([Required, FromQuery] UserEmailDTO dto)
        {
            return await _handler.GetRegistrationStep(dto.Email);
        }
        
        [HttpPost("member/email")]
        public async Task<Response<UserRegistrationStepDTO<AuthInfoDTO>>> SignUpMemberEmail([Required, FromBody] SignUpMemberDTO dto)
        {
            return await _handler.SignUpMemberEmail(dto);
        }
        
        [HttpPost("member/google")]
        public async Task<Response<UserRegistrationStepDTO<AuthInfoDTO>>> SignUpMemberGoogle([Required, FromBody] SignUpMemberGoogleDTO dto)
        {
            return await _handler.SignUpMemberGoogle(dto);
        }
        
        [HttpPost("admin/email")]
        public async Task<Response<UserRegistrationStepDTO<AuthInfoDTO>>> SignUp([Required, FromBody] SignUpDTO dto)
        {
            return await _handler.SignUp(dto.Email, dto.Password);
        }
        
        [HttpPost("admin/google")]
        public async Task<Response<UserRegistrationStepDTO<AuthInfoDTO>>> SignUpGoogle([Required, FromBody] GoogleTokenDTO dto)
        {
            return await _handler.SignUpGoogle(dto.Token);
        }
        
        [HttpPut("admin/confirm")]
        public async Task<Response<UserRegistrationStepDTO>> SignUpConfirm([Required, FromBody] SignUpCodeDTO dto)
        {
            return await _handler.SignUpConfirm(dto.Code, User);
        }
        
        [HttpPut("admin/username")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Response<UserRegistrationStepDTO<UserDTO>>> UpdateCurrentUser([FromBody] UserUpdateDTO dto)
        {
            return await _handler.SignUpUsername(dto, User);
        }
        
        [HttpPut("admin/agreement")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Response<UserRegistrationStepDTO>> SignUpAgreement()
        {
            return await _handler.SignUpAgreement(User);
        }
        
        [HttpPost("admin/company")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Response<UserRegistrationStepDTO<CompanyDTO>>> CreateCompany([Required, FromBody] CreateCompanyDTO dto)
        {
            return await _handler.SignUpCompany(dto, User);
        }
        
        [HttpPost("admin/team")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Response<UserRegistrationStepDTO<TeamDTO>>> CreateTeam([Required, FromBody] CreateTeamDTO dto)
        {
            return await _handler.SignUpTeam(dto, User);
        }
        
        [HttpPost("admin/project")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Response<UserRegistrationStepDTO<ProjectDTO>>> CreateProject([Required, FromBody] CreateProjectDTO dto)
        {
            return await _handler.SignUpProject(dto, User);
        }
        
        [HttpPut("admin/done")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Response<UserRegistrationStepDTO>> SignUpDone()
        {
            return await _handler.SignUpDone(User);
        }
    }
}