using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapper;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Providers.Codes;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Companies;
using Squadio.BLL.Services.Rabbit;
using Squadio.BLL.Services.Users;
using Squadio.Common.Models.Email;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.ChangePassword;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.SignUp;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.TeamsUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Invites;
using Squadio.DTO.Users;

namespace Squadio.BLL.Services.Teams.Implementation
{
    public class TeamInvitesService : ITeamInvitesService
    {
        private readonly IInvitesRepository _repository;
        private readonly ICompanyInvitesService _companyInvitesService;
        private readonly IRabbitService _rabbitService;
        private readonly ITeamsRepository _teamsRepository;
        private readonly ITeamsUsersRepository _teamsUsersRepository;
        private readonly ICodeProvider _codeProvider;
        private readonly IUsersService _usersService;
        private readonly IUsersProvider _usersProvider;
        private readonly ISignUpRepository _signUpRepository;
        private readonly IChangePasswordRequestRepository _changePasswordRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TeamInvitesService> _logger;
        
        public TeamInvitesService(IInvitesRepository repository
            , ICompanyInvitesService companyInvitesService
            , IRabbitService rabbitService
            , ITeamsRepository teamsRepository
            , ITeamsUsersRepository teamsUsersRepository
            , ICodeProvider codeProvider
            , IUsersService usersService
            , IUsersProvider usersProvider
            , ISignUpRepository signUpRepository
            , IChangePasswordRequestRepository changePasswordRepository
            , IMapper mapper
            , ILogger<TeamInvitesService> logger)
        {
            _repository = repository;
            _companyInvitesService = companyInvitesService;
            _rabbitService = rabbitService;
            _teamsRepository = teamsRepository;
            _teamsUsersRepository = teamsUsersRepository;
            _codeProvider = codeProvider;
            _usersService = usersService;
            _usersProvider = usersProvider;
            _signUpRepository = signUpRepository;
            _changePasswordRepository = changePasswordRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        
        public async Task<Response> CreateInvite(Guid teamId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true)
        {
            var teamUser = await _teamsUsersRepository.GetTeamUser(teamId, authorId);
            if (teamUser == null || teamUser?.Status == UserStatus.Member)
            {
                return new PermissionDeniedErrorResponse<IEnumerable<InviteDTO>>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied,
                    }
                });
            }
            
            foreach (var email in dto.Emails)
            {
                var createInviteResult = await CreateInviteToTeam(
                    teamUser.TeamId,
                    email,
                    authorId);
                if (createInviteResult != null && sendInvites)
                {
                    await SendTeamInvite(
                        createInviteResult.Item1,
                        email,
                        teamUser.User.Name,
                        teamUser.Team.Name,
                        createInviteResult.Item2);
                }
            }

            return new Response();
        }

        public async Task<Response> CancelInvite(Guid teamId, Guid authorId, CancelInvitesDTO dto)
        {
            var teamUser = await _teamsUsersRepository.GetTeamUser(teamId, authorId);
            if (teamUser == null || teamUser?.Status == UserStatus.Member)
            {
                return new PermissionDeniedErrorResponse<IEnumerable<InviteDTO>>(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied,
                    }
                });
            }

            await _teamsUsersRepository.DeleteTeamUsers(teamId, dto.Emails);

            await _repository.ActivateInvites(teamId, dto.Emails);

            return new Response();
        }

        public async Task<Response> AcceptInvite(Guid userId, string code)
        {
            var invite = await _repository.GetInviteByCode(code);

            if (invite == null
                || invite?.Activated == true
                || invite?.Code.ToUpper() != code.ToUpper())
            {
                return new SecurityErrorResponse(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.InviteInvalid,
                        Message = ErrorMessages.Security.InviteInvalid
                    }
                });
            }

            var user = (await _usersProvider.GetById(userId)).Data;
            if (user == null)
            {
                return new BusinessConflictErrorResponse(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Business.UserDoesNotExists,
                        Message = ErrorMessages.Business.UserDoesNotExists
                    }
                });
            }

            if (user.Email.ToUpper() != invite.Email.ToUpper())
            {
                return new PermissionDeniedErrorResponse(new[]
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied
                    }
                });
            }

            var result = await AcceptInvite(invite.EntityId, user.Id);
            await _repository.ActivateInvite(invite.Id);

            return result;
        }

        public async Task<Response> AcceptInvite(Guid teamId, Guid userId)
        {
            var team = await _teamsRepository.GetById(teamId);
            if (team == null)
            {
                return new BusinessConflictErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = ErrorFields.Team.Id
                    }
                });
            }

            var companyAccept = await _companyInvitesService.AcceptInvite(team.CompanyId, userId);
            if (!companyAccept.IsSuccess) return companyAccept;

            var teamUser = await _teamsUsersRepository.GetTeamUser(team.Id, userId);
            if (teamUser == null)
                await _teamsUsersRepository.AddTeamUser(team.Id, userId, UserStatus.Member);
            else if (teamUser.Status == UserStatus.Pending)
                await _teamsUsersRepository.ChangeStatusTeamUser(team.Id, userId, UserStatus.Member);

            return new Response();
        }

        private async Task<Tuple<InviteModel, bool>> CreateInviteToTeam(Guid teamId, string email, Guid authorId)
        {
            var code = _codeProvider.GenerateCodeAsGuid();
            InviteModel invite = null;
            var isAlreadyRegistered = true;

            var user = (await _usersProvider.GetByEmail(email)).Data;
            if (user == null)
            {
                var createUserDTO = new UserCreateDTO()
                {
                    Email = email,
                    Step = RegistrationStep.New,
                    Status = UserStatus.Member
                };
                user = (await _usersService.CreateUser(createUserDTO)).Data;

                var signUpStep = await _signUpRepository.GetRegistrationStepByUserId(user.Id);
                if (signUpStep.Step == RegistrationStep.New)
                {
                    isAlreadyRegistered = false;
                    await _changePasswordRepository.AddRequest(user.Id, code);
                }

                var teamUser = await _teamsUsersRepository.GetTeamUser(teamId, user.Id);
                if (teamUser != null)
                {
                    return null;
                }

                await _teamsUsersRepository.AddTeamUser(teamId, user.Id, UserStatus.Pending);

                invite = new InviteModel
                {
                    Email = email,
                    Activated = false,
                    CreatedDate = DateTime.UtcNow,
                    Code = code,
                    EntityId = teamId,
                    EntityType = EntityType.Team,
                    CreatorId = authorId
                };

                invite = await _repository.CreateInvite(invite);
            }

            return new Tuple<InviteModel, bool>(invite, isAlreadyRegistered);
        }

        private async Task SendTeamInvite(InviteModel model, string email, string authorName, string teamName, bool isAlreadyRegistered)
        {
            try
            {
                if (model != null)
                {
                    await _rabbitService.Send(new InviteToTeamEmailModel()
                    {
                        To = email,
                        AuthorName = authorName,
                        Code = model.Code,
                        TeamName = teamName,
                        IsAlreadyRegistered = isAlreadyRegistered
                    });
                }
                else
                {
                    await _rabbitService.Send(new AddToTeamEmailModel()
                    {
                        To = email,
                        AuthorName = authorName,
                        TeamName = teamName
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{email}] : {ex.Message}");
            }
        }
    }
}