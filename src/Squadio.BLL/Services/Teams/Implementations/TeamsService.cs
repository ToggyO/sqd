using System;
using System.Threading.Tasks;
using AutoMapper;
using Squadio.BLL.Services.Membership;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.TeamsUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Teams;
using Squadio.DTO.Models.Invites;
using Squadio.DTO.Models.Teams;

namespace Squadio.BLL.Services.Teams.Implementations
{
    public class TeamsService : ITeamsService
    {
        private readonly ITeamsRepository _repository;
        private readonly ITeamsUsersRepository _teamsUsersRepository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly IMembershipService _membershipService;
        private readonly IMapper _mapper;

        public TeamsService(ITeamsRepository repository
            , ITeamsUsersRepository teamsUsersRepository
            , ICompaniesUsersRepository companiesUsersRepository
            , IMembershipService membershipService
            , IMapper mapper)
        {
            _repository = repository;
            _teamsUsersRepository = teamsUsersRepository;
            _companiesUsersRepository = companiesUsersRepository;
            _membershipService = membershipService;
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

            await _teamsUsersRepository.AddTeamUser(entity.Id, userId, MembershipStatus.SuperAdmin);
            
            await _membershipService.InviteUsers(entity.Id, InviteEntityType.Team, userId, dto.Emails, sendInvites);

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

            if (teamUser.Status != MembershipStatus.SuperAdmin)
            {
                return new ForbiddenErrorResponse<TeamDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.Forbidden,
                        Message = ErrorMessages.Security.Forbidden
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
            if (teamUser == null || teamUser.Status != MembershipStatus.SuperAdmin)
            {
                return new ForbiddenErrorResponse<TeamDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.Forbidden,
                        Message = ErrorMessages.Security.Forbidden
                    }
                }); 
            }

            var companyUser = await _companiesUsersRepository.GetCompanyUser(teamUser.Team.CompanyId, userId);
            if (companyUser == null || companyUser.Status != MembershipStatus.SuperAdmin)
            {
                return new ForbiddenErrorResponse<TeamDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.Forbidden,
                        Message = ErrorMessages.Security.Forbidden
                    }
                }); 
            }

            var entity = await _repository.Delete(teamId);
            return new Response<TeamDTO>
            {
                Data = _mapper.Map<TeamModel, TeamDTO>(entity)
            };
        }

    }
}