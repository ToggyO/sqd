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
        Task<Response> SignUpMemberEmail(SignUpMemberDTO dto);
        Task<Response> SignUpMemberGoogle(SignUpMemberGoogleDTO dto);
        Task<Response> SignUp(string email, string password);
        Task<Response> SignUpGoogle(string googleToken);
        Task<Response<UserRegistrationStepDTO>> SignUpConfirm(Guid userId, string code);
        Task<Response<UserRegistrationStepDTO<UserDTO>>> SignUpUsername(Guid userId, UserUpdateDTO updateDTO);
        Task<Response<UserRegistrationStepDTO>> SignUpAgreement(Guid userId);
        Task<Response<UserRegistrationStepDTO<CompanyDTO>>> SignUpCompany(Guid userId, CreateCompanyDTO dto);
        Task<Response<UserRegistrationStepDTO<TeamDTO>>> SignUpTeam(Guid userId, CreateTeamDTO dto);
        Task<Response<UserRegistrationStepDTO<ProjectDTO>>> SignUpProject(Guid userId, CreateProjectDTO dto);
        Task<Response<UserRegistrationStepDTO>> SignUpDone(Guid userId);
    }
}