using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.SignUp
{
    public interface ISignUpService
    {
        Task<Response> SignUp(string email);
        Task<Response<UserDTO>> SignUpGoogle(string googleToken);
        Task<Response<UserDTO>> SignUpPassword(string email, string code, string password);
        Task<Response<UserDTO>> SignUpUsername(Guid id, UserUpdateDTO updateDTO);
        Task<Response<CompanyDTO>> SignUpCompany(Guid userId, CreateCompanyDTO dto);
        Task<Response<TeamDTO>> SignUpTeam(Guid userId, CreateTeamDTO dto);
        Task<Response<ProjectDTO>> SignUpProject(Guid userId, CreateProjectDTO dto);
        Task<Response> SignUpDone(Guid userId);
    }
}