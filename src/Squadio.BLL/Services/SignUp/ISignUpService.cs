using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.SignUp;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.SignUp
{
    public interface ISignUpService
    {
        Task<Response<UserDTO>> SignUpMemberEmail(SignUpMemberDTO dto);
        Task<Response<UserDTO>> SignUpMemberGoogle(SignUpMemberGoogleDTO dto);
        Task<Response<UserDTO>> SignUp(string email, string password);
        Task<Response<UserDTO>> SignUpGoogle(string googleToken);
        //Task<Response<UserDTO>> SignUpPassword(string email, string code, string password);
        Task<Response> SignUpConfirm(Guid userId, string code);
        Task<Response<UserDTO>> SignUpUsername(Guid userId, UserUpdateDTO updateDTO);
        Task<Response> SignUpAgreement(Guid userId);
        Task<Response<CompanyDTO>> SignUpCompany(Guid userId, CreateCompanyDTO dto);
        Task<Response<TeamDTO>> SignUpTeam(Guid userId, CreateTeamDTO dto);
        Task<Response<ProjectDTO>> SignUpProject(Guid userId, CreateProjectDTO dto);
        Task<Response> SignUpDone(Guid userId);
    }
}