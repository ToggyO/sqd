using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Squadio.API.Filters;
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
    [AuthorizationFilter]
    [Route("api/signup")]
    public class SignUpController : ControllerBase
    {
        private readonly ISignUpHandler _handler;

        public SignUpController(ISignUpHandler handler)
        {
            _handler = handler;
        }
        
        /*
        [HttpGet("step")]
        public async Task<Response<UserRegistrationStepDTO>> GetStatus([Required, FromQuery] UserEmailDTO dto)
        {
            return await _handler.GetRegistrationStep(dto.Email);
        }
        */
        
        [HttpPost("member/email")]
        [AllowAnonymous]
        public async Task<Response<AuthInfoDTO>> SignUpMemberEmail([Required, FromBody] SignUpMemberDTO dto)
        {
            return await _handler.SignUpMemberEmail(dto);
        }
        
        [HttpPost("member/google")]
        [AllowAnonymous]
        public async Task<Response<AuthInfoDTO>> SignUpMemberGoogle([Required, FromBody] SignUpMemberGoogleDTO dto)
        {
            return await _handler.SignUpMemberGoogle(dto);
        }
        
        [HttpPost("admin/email")]
        [AllowAnonymous]
        public async Task<Response<AuthInfoDTO>> SignUp([Required, FromBody] SignUpDTO dto)
        {
            return await _handler.SignUp(dto.Email, dto.Password);
        }
        
        [HttpPost("admin/google")]
        [AllowAnonymous]
        public async Task<Response<AuthInfoDTO>> SignUpGoogle([Required, FromBody] GoogleTokenDTO dto)
        {
            return await _handler.SignUpGoogle(dto.Token);
        }
        
        [HttpPut("admin/confirm")]
        public async Task<Response<UserRegistrationStepDTO>> SignUpConfirm([Required, FromBody] SignUpCodeDTO dto)
        {
            return await _handler.SignUpConfirm(dto.Code, User);
        }
        
        [HttpPut("admin/username")]
        public async Task<Response<UserRegistrationStepDTO<UserDTO>>> UpdateCurrentUser([FromBody] UserUpdateDTO dto)
        {
            return await _handler.SignUpUsername(dto, User);
        }
        
        [HttpPost("admin/company")]
        public async Task<Response<UserRegistrationStepDTO<CompanyDTO>>> CreateCompany([Required, FromBody] CreateCompanyDTO dto)
        {
            return await _handler.SignUpCompany(dto, User);
        }
        
        [HttpPost("admin/team")]
        public async Task<Response<UserRegistrationStepDTO<TeamDTO>>> CreateTeam([Required, FromBody] CreateTeamDTO dto)
        {
            return await _handler.SignUpTeam(dto, User);
        }
        
        [HttpGet("admin/team/invites")]
        public async Task<Response<IEnumerable<string>>> GetTeamInvites()
        {
            return await _handler.GetTeamInvites(User);
        }
        
        [HttpPost("admin/project")]
        public async Task<Response<UserRegistrationStepDTO<ProjectDTO>>> CreateProject([Required, FromBody] CreateProjectDTO dto)
        {
            return await _handler.SignUpProject(dto, User);
        }
        
        [HttpPut("admin/done")]
        public async Task<Response<UserRegistrationStepDTO>> SignUpDone()
        {
            return await _handler.SignUpDone(User);
        }
    }
}