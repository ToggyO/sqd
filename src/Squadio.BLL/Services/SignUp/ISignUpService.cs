using System;
using System.Threading.Tasks;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.SignUp
{
    public interface ISignUpService
    {
        Task SignUp(string email);
        Task<UserDTO> SignUpGoogle(string googleToken);
        Task<UserDTO> SignUpPassword(string email, string code, string password);
        Task<UserDTO> SignUpUsername(Guid id, UserUpdateDTO updateDTO);
        Task<CompanyDTO> SignUpCompany(Guid userId, CreateCompanyDTO dto);
        Task<TeamDTO> SignUpTeam(Guid userId, CreateTeamDTO dto);
        Task<ProjectDTO> SignUpProject(Guid userId, CreateProjectDTO dto);
        Task SignUpDone(Guid userId);
    }
}