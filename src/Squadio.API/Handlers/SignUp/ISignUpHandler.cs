using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.SignUp;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.SignUp
{
    public interface ISignUpHandler
    {
        Task<Response<SignUpStepDTO>> GetRegistrationStep(string email);
        Task<Response<SignUpStepDTO>> GetRegistrationStep(ClaimsPrincipal claims);
        Task<Response<AuthInfoDTO>> SignUpMemberEmail(SignUpMemberDTO dto);
        Task<Response<AuthInfoDTO>> SignUpMemberGoogle(SignUpMemberGoogleDTO dto);
        Task<Response<AuthInfoDTO>> SignUp(string email, string password);
        Task<Response<AuthInfoDTO>> SignUpGoogle(string googleToken);
        Task<Response<SignUpStepDTO>> SendNewCode(string email);
        Task<Response<SignUpStepDTO>> SendNewCode(ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO>> SignUpConfirm(string code, ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO<UserDTO>>> SignUpUsername(UserUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO<CompanyDTO>>> SignUpCompany(CreateCompanyDTO dto, ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO<TeamDTO>>> SignUpTeam(CreateTeamDTO dto, ClaimsPrincipal claims);
        Task<Response<IEnumerable<string>>> GetTeamInvites(ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO<ProjectDTO>>> SignUpProject(CreateProjectDTO dto, ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO>> SignUpDone(ClaimsPrincipal claims);
    }
}