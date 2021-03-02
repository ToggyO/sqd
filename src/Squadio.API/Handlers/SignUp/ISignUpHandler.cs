using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Auth;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Projects;
using Squadio.DTO.Models.SignUp;
using Squadio.DTO.Models.Teams;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;
using Squadio.DTO.Projects;

namespace Squadio.API.Handlers.SignUp
{
    public interface ISignUpHandler
    {
        Task<Response> SimpleSignUp(SignUpSimpleDTO dto);
        Task<Response<SignUpStepDTO>> GetRegistrationStep(ClaimsPrincipal claims);
        Task<Response<AuthInfoDTO>> SignUpMemberEmail(SignUpMemberDTO dto);
        // Task<Response<AuthInfoDTO>> SignUpMemberGoogle(SignUpMemberGoogleDTO dto);
        Task<Response<AuthInfoDTO>> SignUp(string email, string password);
        // Task<Response<AuthInfoDTO>> SignUpGoogle(string googleToken);
        Task<Response<SignUpStepDTO>> SendNewCode(ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO>> SignUpConfirm(string code, ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO<UserDTO>>> SignUpUsername(UserUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO<CompanyDTO>>> SignUpCompany(CompanyCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO<TeamDTO>>> SignUpTeam(TeamCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<IEnumerable<string>>> GetTeamInvites(ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO<ProjectDTO>>> SignUpProject(ProjectCreateDTO dto, ClaimsPrincipal claims);
        Task<Response<SignUpStepDTO>> SignUpDone(ClaimsPrincipal claims);
    }
}