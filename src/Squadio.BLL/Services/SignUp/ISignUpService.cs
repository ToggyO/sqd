﻿using System;
 using System.Threading.Tasks;
 using Squadio.Common.Models.Responses;
 using Squadio.DTO.Models.Companies;
 using Squadio.DTO.Models.Projects;
 using Squadio.DTO.Models.SignUp;
 using Squadio.DTO.Models.Teams;
 using Squadio.DTO.Models.Users;
 using Squadio.DTO.Models.Users.Settings;
 using Squadio.DTO.Projects;

 namespace Squadio.BLL.Services.SignUp
{
    public interface ISignUpService
    {
        Task<Response> SimpleSignUp(SignUpSimpleDTO dto);
        Task<Response<UserDTO>> SignUpMemberEmail(SignUpMemberDTO dto);
        // Task<Response> SignUpMemberGoogle(SignUpMemberGoogleDTO dto);
        Task<Response> SignUp(string email, string password);
        // Task<Response> SignUpGoogle(string googleToken);
        Task<Response<SignUpStepDTO>> SendNewCode(Guid userId);
        Task<Response<SignUpStepDTO>> SignUpConfirm(Guid userId, string code);
        Task<Response<SignUpStepDTO<UserDTO>>> SignUpUsername(Guid userId, UserUpdateDTO updateDTO);
        Task<Response<SignUpStepDTO<CompanyDTO>>> SignUpCompany(Guid userId, CompanyCreateDTO dto);
        Task<Response<SignUpStepDTO<TeamDTO>>> SignUpTeam(Guid userId, TeamCreateDTO dto);
        Task<Response<SignUpStepDTO<ProjectDTO>>> SignUpProject(Guid userId, ProjectCreateDTO dto);
        Task<Response<SignUpStepDTO>> SignUpDone(Guid userId);
    }
}