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
        Task<Response<SignUpStepDTO>> SendNewCode(string email);
        Task<Response<SignUpStepDTO>> SignUpConfirm(Guid userId, string code);
        Task<Response<SignUpStepDTO<UserDTO>>> SignUpUsername(Guid userId, UserUpdateDTO updateDTO);
        Task<Response<SignUpStepDTO<CompanyDTO>>> SignUpCompany(Guid userId, CreateCompanyDTO dto);
        Task<Response<SignUpStepDTO<TeamDTO>>> SignUpTeam(Guid userId, CreateTeamDTO dto);
        Task<Response<SignUpStepDTO<ProjectDTO>>> SignUpProject(Guid userId, CreateProjectDTO dto);
        Task<Response<SignUpStepDTO>> SignUpDone(Guid userId);
    }
}