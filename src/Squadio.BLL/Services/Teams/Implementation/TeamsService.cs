using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Services.Invites;
using Squadio.BLL.Services.Projects;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.TeamsUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Teams;
using Squadio.DTO.Companies;
using Squadio.DTO.Invites;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Services.Teams.Implementation
{
    public class TeamsService : ITeamsService
    {
        private readonly ITeamsRepository _repository;
        private readonly ITeamsUsersRepository _teamsUsersRepository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly IProjectsService _projectsService;
        private readonly IInvitesService _invitesService;
        private readonly IMapper _mapper;

        public TeamsService(ITeamsRepository repository
            , ITeamsUsersRepository teamsUsersRepository
            , ICompaniesUsersRepository companiesUsersRepository
            , IProjectsService projectsService
            , IInvitesService invitesService
            , IMapper mapper)
        {
            _repository = repository;
            _teamsUsersRepository = teamsUsersRepository;
            _companiesUsersRepository = companiesUsersRepository;
            _projectsService = projectsService;
            _invitesService = invitesService;
            _mapper = mapper;
        }

        public async Task<Response<TeamDTO>> Create(Guid userId, Guid companyId, TeamCreateDTO dto, bool sendInvites = true)
        {
            var entity = new TeamModel
            {
                Name = dto.Name,
                CompanyId = companyId,
                CreatedDate = DateTime.UtcNow,
                ColorHex = dto.ColorHex,
                CreatorId = userId
            };

            entity = await _repository.Create(entity);

            await _teamsUsersRepository.AddTeamUser(entity.Id, userId, UserStatus.SuperAdmin);

            await _invitesService.InviteToTeam(
                entity.Id,
                userId,
                new CreateInvitesDTO
                {
                    Emails = dto.Emails
                }, 
                sendInvites);

            var result = _mapper.Map<TeamModel, TeamDTO>(entity);
            return new Response<TeamDTO>
            {
                Data = result
            };
        }

        public async Task<Response<TeamDTO>> Update(Guid teamId, Guid userId, TeamUpdateDTO dto)
        {
            
            var teamEntity = await _repository.GetById(teamId);
            
            if (teamEntity == null)
            {
                return new BusinessConflictErrorResponse<TeamDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = "teamId"
                    }
                });
            }
            
            var teamUser = await _teamsUsersRepository.GetTeamUser(teamId, userId);
            
            if (teamUser == null)
            {
                return new BusinessConflictErrorResponse<TeamDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = "userId"
                    }
                });
            }

            if (teamUser.Status != UserStatus.SuperAdmin)
            {
                return new PermissionDeniedErrorResponse<TeamDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied
                    }
                }); 
            }
            
            teamEntity.Name = dto.Name;
            teamEntity.ColorHex = dto.ColorHex;
            
            teamEntity = await _repository.Update(teamEntity);
            
            var result = _mapper.Map<TeamModel, TeamDTO>(teamEntity);
            
            return new Response<TeamDTO>
            {
                Data = result
            };
        }

        public async Task<Response<TeamDTO>> Delete(Guid teamId, Guid userId)
        {
            var teamUser = await _teamsUsersRepository.GetTeamUser(teamId, userId);
            if (teamUser == null || teamUser.Status != UserStatus.SuperAdmin)
            {
                return new PermissionDeniedErrorResponse<TeamDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied
                    }
                }); 
            }

            var companyUser = await _companiesUsersRepository.GetCompanyUser(teamUser.Team.CompanyId, userId);
            if (companyUser == null || companyUser.Status != UserStatus.SuperAdmin)
            {
                return new PermissionDeniedErrorResponse<TeamDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied
                    }
                }); 
            }

            var entity = await _repository.Delete(teamId);
            return new Response<TeamDTO>
            {
                Data = _mapper.Map<TeamModel, TeamDTO>(entity)
            };
        }

        public async Task<Response> DeleteUserFromTeam(Guid teamId, Guid removeUserId, Guid currentUserId)
        {
            var currentTeamUser = await _teamsUsersRepository.GetTeamUser(teamId, currentUserId);

            if (currentTeamUser == null || currentTeamUser?.Status != UserStatus.SuperAdmin)
            {
                return new PermissionDeniedErrorResponse(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.PermissionDenied,
                        Message = ErrorMessages.Security.PermissionDenied
                    }
                }); 
            }

            return await DeleteUserFromTeam(teamId, removeUserId);
        }

        public async Task<Response> LeaveTeam(Guid teamId, Guid userId)
        {
            return await DeleteUserFromTeam(teamId, userId);
        }

        public async Task<Response> DeleteUserFromTeamsByCompanyId(Guid companyId, Guid removeUserId)
        {
            var teams = await _repository.GetTeams(new PageModel()
            {
                Page = 1,
                PageSize = 1000
            }, companyId);
            
            foreach (var team in teams.Items)
            {
                await DeleteUserFromTeam(team.Id, removeUserId);
            }
            
            return new Response();
        }
        
        private async Task<Response> DeleteUserFromTeam(Guid teamId, Guid removeUserId)
        {
            await _teamsUsersRepository.DeleteTeamUser(teamId, removeUserId);
            await _projectsService.DeleteUserFromProjectsByTeamId(teamId, removeUserId);
            
            return new Response();
        }
    }
}