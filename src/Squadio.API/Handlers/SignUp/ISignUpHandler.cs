using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Auth;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.SignUp
{
    public interface ISignUpHandler
    {
        Task<Response<UserRegistrationStepDTO>> GetRegistrationStep(string email);
        Task<Response> SignUp(string email);
        Task<Response<UserDTO>> SignUpGoogle(string googleToken);
        Task<Response<AuthInfoDTO>> SignUpPassword(UserSetPasswordDTO dto);
        Task<Response<UserDTO>> SignUpUsername(UserUpdateDTO dto, ClaimsPrincipal claims);
        Task<Response<CompanyDTO>> SignUpCompany(CreateCompanyDTO dto, ClaimsPrincipal claims);
        Task<Response<TeamDTO>> SignUpTeam(CreateTeamDTO dto, ClaimsPrincipal claims);
        Task<Response<ProjectDTO>> SignUpProject(CreateProjectDTO dto, ClaimsPrincipal claims);
        Task<Response> SignUpDone(ClaimsPrincipal claims);
    }
}