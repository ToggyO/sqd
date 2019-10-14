﻿using System;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Mapper;
using Microsoft.Extensions.Options;
using Squadio.BLL.Services.Companies;
using Squadio.BLL.Services.Email;
using Squadio.BLL.Services.Projects;
using Squadio.BLL.Services.Teams;
using Squadio.BLL.Services.Users;
using Squadio.Common.Exceptions.BusinessLogicExceptions;
using Squadio.Common.Exceptions.SecurityExceptions;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;
using Squadio.DAL.Repository.Users;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.SignUp.Implementation
{
    public class SignUpService : ISignUpService
    {
        private readonly IUsersRepository _repository;
        private readonly IEmailService<PasswordSetEmailModel> _passwordSetMailService;
        private readonly IOptions<GoogleSettings> _googleSettings;
        private readonly IUsersService _usersService;
        private readonly ICompaniesService _companiesService;
        private readonly ITeamsService _teamsService;
        private readonly IProjectsService _projectsService;
        private readonly IMapper _mapper;

        public SignUpService(IUsersRepository repository
            , IEmailService<PasswordSetEmailModel> passwordSetMailService
            , IOptions<GoogleSettings> googleSettings
            , IUsersService usersService
            , ICompaniesService companiesService
            , ITeamsService teamsService
            , IProjectsService projectsService
            , IMapper mapper
        )
        {
            _repository = repository;
            _passwordSetMailService = passwordSetMailService;
            _googleSettings = googleSettings;
            _usersService = usersService;
            _companiesService = companiesService;
            _teamsService = teamsService;
            _projectsService = projectsService;
            _mapper = mapper;
        }

        public async Task SignUp(string email)
        {
            var user = await _repository.GetByEmail(email);
            if (user != null)
                throw new Exception("Email already used");

            var code = _usersService.GenerateCode();

            await _passwordSetMailService.Send(new PasswordSetEmailModel
            {
                Code = code,
                To = email
            });

            user = new UserModel
            {
                Email = email,
                CreatedDate = DateTime.UtcNow
            };

            user = await _repository.Create(user);

            await _repository.AddPasswordRequest(user.Id, code);

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.New);
        }

        public async Task<UserDTO> SignUpGoogle(string googleToken)
        {
            var infoFromGoogleToken = await GoogleJsonWebSignature.ValidateAsync(googleToken);

            if ((string) infoFromGoogleToken.Audience != _googleSettings.Value.ClientId)
                throw new SecurityException("security_error", "Incorrect google token");

            var user = await _repository.GetByEmail(infoFromGoogleToken.Email);
            if (user != null)
                throw new BusinessLogicException("business_conflict", "User already exist");

            user = new UserModel
            {
                Name = $"{infoFromGoogleToken.Name}",
                Email = infoFromGoogleToken.Email,
                CreatedDate = DateTime.Now
            };
            user = await _repository.Create(user);

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.EmailConfirmed);

            var code = _usersService.GenerateCode();
            await _passwordSetMailService.Send(new PasswordSetEmailModel
            {
                Code = code,
                To = user.Email
            });
            await _repository.AddPasswordRequest(user.Id, code);

            var result = _mapper.Map<UserModel, UserDTO>(user);
            return result;
        }

        public async Task<UserDTO> SignUpPassword(string email, string code, string password)
        {
            var step = await _repository.GetRegistrationStepByEmail(email);

            if (step.Step >= RegistrationStep.PasswordEntered)
                throw new Exception("DALSHE!");

            var user = await _usersService.SetPassword(email, code, password);

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.PasswordEntered);

            return user;
        }

        public async Task<UserDTO> SignUpUsername(Guid id, UserUpdateDTO updateDTO)
        {
            var step = await _repository.GetRegistrationStepByUserId(id);

            if (step.Step >= RegistrationStep.PasswordEntered)
                throw new Exception("DALSHE!");

            var user = await _usersService.UpdateUser(id, updateDTO);

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.UsernameEntered);

            return user;
        }

        public async Task<CompanyDTO> SignUpCompany(Guid userId, CreateCompanyDTO dto)
        {
            var company = await _companiesService.Create(userId, dto);

            await _repository.SetRegistrationStep(userId, RegistrationStep.CompanyCreated);

            return company;
        }

        public async Task<TeamDTO> SignUpTeam(Guid userId, CreateTeamDTO dto)
        {
            var team = await _teamsService.Create(userId, dto);

            await _repository.SetRegistrationStep(userId, RegistrationStep.TeamCreated);

            return team;
        }

        public async Task<ProjectDTO> SignUpProject(Guid userId, CreateProjectDTO dto)
        {
            var team = await _projectsService.Create(userId, dto);

            await _repository.SetRegistrationStep(userId, RegistrationStep.ProjectCreated);

            return team;
        }

        public async Task SignUpDone(Guid userId)
        {
            await _repository.SetRegistrationStep(userId, RegistrationStep.Done);
        }
    }
}