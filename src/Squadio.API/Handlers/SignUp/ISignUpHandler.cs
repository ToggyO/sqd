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
        Task<Response<UserRegistrationStepDTO>> GetRegistrationStep(string email);
        Task<Response<AuthInfoDTO>> SignUpMemberEmail(SignUpMemberDTO dto);
        Task<Response<AuthInfoDTO>> SignUpMemberGoogle(SignUpMemberGoogleDTO dto);
        Task<Response<AuthInfoDTO>> SignUp(string email, string password);
        Task<Response<AuthInfoDTO>> SignUpGoogle(string googleToken);
        Task<Response<UserRegistrationStepDTO>> SignUpConfirm(string code, ClaimsPrincipal claims);
        Task<Response<UserRegistrationStepDTO<UserDTO>>> SignUpUsername(UserUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response<UserRegistrationStepDTO<CompanyDTO>>> SignUpCompany(CreateCompanyDTO dto, ClaimsPrincipal claims);
        Task<Response<UserRegistrationStepDTO<TeamDTO>>> SignUpTeam(CreateTeamDTO dto, ClaimsPrincipal claims);
        Task<Response<UserRegistrationStepDTO<ProjectDTO>>> SignUpProject(CreateProjectDTO dto, ClaimsPrincipal claims);
        Task<Response<UserRegistrationStepDTO>> SignUpDone(ClaimsPrincipal claims);
    }
}