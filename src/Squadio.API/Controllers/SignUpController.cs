using System;
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
            // TODO: Remove obsolete endpoints
            
            _handler = handler;
        }

        /// <summary>
        /// Get registration step of current user
        /// </summary>
        [HttpGet("step")]
        public async Task<Response<SignUpStepDTO>> GetStatus()
        {
            return await _handler.GetRegistrationStep(User);
        }
        
        /// <summary>
        /// Confirm email, using invite
        /// </summary>
        [HttpPost("member/email")]
        [AllowAnonymous]
        public async Task<Response<AuthInfoDTO>> SignUpMemberEmail([Required, FromBody] SignUpMemberDTO dto)
        {
            return await _handler.SignUpMemberEmail(dto);
        }
        
        /// <summary>
        /// Confirm email, set username, using google token and invite
        /// </summary>
        [HttpPost("member/google")]
        [AllowAnonymous]
        public async Task<Response<AuthInfoDTO>> SignUpMemberGoogle([Required, FromBody] SignUpMemberGoogleDTO dto)
        {
            return await _handler.SignUpMemberGoogle(dto);
        }
        
        /// <summary>
        /// Create new account and send confirmation email
        /// </summary>
        [HttpPost("admin/email")]
        [AllowAnonymous]
        public async Task<Response<AuthInfoDTO>> SignUp([Required, FromBody] SignUpDTO dto)
        {
            return await _handler.SignUp(dto.Email, dto.Password);
        }
        
        /// <summary>
        /// Create new account
        /// </summary>
        [HttpPost("admin/google")]
        [AllowAnonymous]
        public async Task<Response<AuthInfoDTO>> SignUpGoogle([Required, FromBody] GoogleTokenDTO dto)
        {
            return await _handler.SignUpGoogle(dto.Token);
        }
        
        /// <summary>
        /// Send new confirmation email and disable previous
        /// </summary>
        [HttpPost("send-new-code")]
        public async Task<Response<SignUpStepDTO>> SendNewCode()
        {
            return await _handler.SendNewCode(User);
        }
        
        /// <summary>
        /// use PUT api/signup/confirm
        /// </summary>
        [Obsolete]
        [HttpPut("admin/confirm")]
        public async Task<Response<SignUpStepDTO>> SignUpConfirmObsolete([Required, FromBody] SignUpCodeDTO dto)
        {
            return await _handler.SignUpConfirm(dto.Code, User);
        }
        
        /// <summary>
        /// Confirm email of current user
        /// </summary>
        [HttpPut("confirm")]
        public async Task<Response<SignUpStepDTO>> SignUpConfirm([Required, FromBody] SignUpCodeDTO dto)
        {
            return await _handler.SignUpConfirm(dto.Code, User);
        }
        
        /// <summary>
        /// use PUT api/signup/username
        /// </summary>
        [Obsolete]
        [HttpPut("member/username")]
        public async Task<Response<SignUpStepDTO<UserDTO>>> SignUpMemberUsernameObsolete([FromBody] UserUpdateDTO dto)
        {
            return await _handler.SignUpUsername(dto, User);
        }
        
        /// <summary>
        /// use PUT api/signup/username
        /// </summary>
        [Obsolete]
        [HttpPut("admin/username")]
        public async Task<Response<SignUpStepDTO<UserDTO>>> SignUpUsernameObsolete([FromBody] UserUpdateDTO dto)
        {
            return await _handler.SignUpUsername(dto, User);
        }
        
        /// <summary>
        /// Set username of current user (also mean that privacy policy and terms was read)
        /// </summary>
        [HttpPut("username")]
        public async Task<Response<SignUpStepDTO<UserDTO>>> UpdateCurrentUser([FromBody] UserUpdateDTO dto)
        {
            return await _handler.SignUpUsername(dto, User);
        }
        
        /// <summary>
        /// use POST api/signup/company
        /// </summary>
        [Obsolete]
        [HttpPost("admin/company")]
        public async Task<Response<SignUpStepDTO<CompanyDTO>>> CreateCompanyObsolete([Required, FromBody] CompanyCreateDTO dto)
        {
            return await _handler.SignUpCompany(dto, User);
        }
        
        /// <summary>
        /// Create new company (and set current user as admin for this company )
        /// </summary>
        [HttpPost("company")]
        public async Task<Response<SignUpStepDTO<CompanyDTO>>> CreateCompany([Required, FromBody] CompanyCreateDTO dto)
        {
            return await _handler.SignUpCompany(dto, User);
        }
        
        /// <summary>
        /// use POST api/signup/team
        /// </summary>
        [Obsolete]
        [HttpPost("admin/team")]
        public async Task<Response<SignUpStepDTO<TeamDTO>>> CreateTeamObsolete([Required, FromBody] TeamCreateDTO dto)
        {
            return await _handler.SignUpTeam(dto, User);
        }
        
        /// <summary>
        /// Create new team, send invites (and set current user as admin for this team)
        /// </summary>
        [HttpPost("team")]
        public async Task<Response<SignUpStepDTO<TeamDTO>>> CreateTeam([Required, FromBody] TeamCreateDTO dto)
        {
            return await _handler.SignUpTeam(dto, User);
        }
        
        /// <summary>
        /// Get emails of invited users (working correctly only in registration)
        /// </summary>
        [HttpGet("team/invites")]
        public async Task<Response<IEnumerable<string>>> GetTeamInvites()
        {
            return await _handler.GetTeamInvites(User);
        }
        
        /// <summary>
        /// use POST api/signup/project
        /// </summary>
        [Obsolete]
        [HttpPost("admin/project")]
        public async Task<Response<SignUpStepDTO<ProjectDTO>>> CreateProjectObsolete([Required, FromBody] CreateProjectDTO dto)
        {
            return await _handler.SignUpProject(dto, User);
        }
        
        /// <summary>
        /// Create new project, send invites (and set current user as admin for this project)
        /// </summary>
        [HttpPost("project")]
        public async Task<Response<SignUpStepDTO<ProjectDTO>>> CreateProject([Required, FromBody] CreateProjectDTO dto)
        {
            return await _handler.SignUpProject(dto, User);
        }
        
        /// <summary>
        /// use PUT api/signup/done
        /// </summary>
        [Obsolete]
        [HttpPut("admin/done")]
        public async Task<Response<SignUpStepDTO>> SignUpDoneObsolete()
        {
            return await _handler.SignUpDone(User);
        }
        
        /// <summary>
        /// User watched video tutorial and ready to use application
        /// </summary>
        [HttpPut("done")]
        public async Task<Response<SignUpStepDTO>> SignUpDone()
        {
            return await _handler.SignUpDone(User);
        }
    }
}