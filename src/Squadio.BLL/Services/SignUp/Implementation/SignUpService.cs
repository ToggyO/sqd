using System;
using System.Net;
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
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
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

        public async Task<Response> SignUp(string email)
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
            
            return new Response();
        }

        public async Task<Response<UserDTO>> SignUpGoogle(string googleToken)
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

            //return result;
            return new Response<UserDTO>();
        }

        public async Task<Response<UserDTO>> SignUpPassword(string email, string code, string password)
        {
            var step = await _repository.GetRegistrationStepByEmail(email);

            if (step.Step >= RegistrationStep.PasswordEntered)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }

            var userResponse = await _usersService.SetPassword(email, code, password);
            var user = userResponse.Data;

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.PasswordEntered);

            //return user;
            return new Response<UserDTO>();
        }

        public async Task<Response<UserDTO>> SignUpUsername(Guid id, UserUpdateDTO updateDTO)
        {
            var step = await _repository.GetRegistrationStepByUserId(id);

            if (step.Step >= RegistrationStep.UsernameEntered)
            {
                return new ErrorResponse<UserDTO>
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }

            var userResponse = await _usersService.UpdateUser(id, updateDTO);
            var user = userResponse.Data;

            await _repository.SetRegistrationStep(user.Id, RegistrationStep.UsernameEntered);

            //return user;
            return new Response<UserDTO>();
        }

        public async Task<Response<CompanyDTO>> SignUpCompany(Guid userId, CreateCompanyDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.CompanyCreated)
            {
                return new ErrorResponse<CompanyDTO>
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }
            
            var company = await _companiesService.Create(userId, dto);

            await _repository.SetRegistrationStep(userId, RegistrationStep.CompanyCreated);

            return company;
        }

        public async Task<Response<TeamDTO>> SignUpTeam(Guid userId, CreateTeamDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.TeamCreated)
            {
                return new ErrorResponse<TeamDTO>
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }
            
            var team = await _teamsService.Create(userId, dto);

            await _repository.SetRegistrationStep(userId, RegistrationStep.TeamCreated);

            //return team;
            return new Response<TeamDTO>();
        }

        public async Task<Response<ProjectDTO>> SignUpProject(Guid userId, CreateProjectDTO dto)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.ProjectCreated)
            {
                return new ErrorResponse<ProjectDTO>
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }
            
            var team = await _projectsService.Create(userId, dto);

            await _repository.SetRegistrationStep(userId, RegistrationStep.ProjectCreated);

            //return team;
            return new Response<ProjectDTO>();
        }

        public async Task<Response> SignUpDone(Guid userId)
        {
            var step = await _repository.GetRegistrationStepByUserId(userId);

            if (step.Step >= RegistrationStep.Done)
            {
                return new ErrorResponse<ProjectDTO>
                {
                    Code = ErrorCodes.Business.InvalidRegistrationStep,
                    Message = ErrorMessages.Business.InvalidRegistrationStep,
                    // TODO: find correct http code for this
                    HttpStatusCode = HttpStatusCode.Conflict
                };
            }
            
            await _repository.SetRegistrationStep(userId, RegistrationStep.Done);
            return new Response();
        }
    }
}