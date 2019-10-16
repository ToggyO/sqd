using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Services.Invites;
using Squadio.Common.Models.Responses;
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
        private readonly IInvitesService _invitesService;
        private readonly IMapper _mapper;

        public TeamsService(ITeamsRepository repository
            , ITeamsUsersRepository teamsUsersRepository
            , IInvitesService invitesService
            , IMapper mapper)
        {
            _repository = repository;
            _teamsUsersRepository = teamsUsersRepository;
            _invitesService = invitesService;
            _mapper = mapper;
        }

        public async Task<Response<TeamDTO>> Create(Guid userId, CreateTeamDTO dto)
        {
            var entity = new TeamModel
            {
                Name = dto.Name,
                CompanyId = dto.CompanyId,
                CreatedDate = DateTime.UtcNow
            };

            entity = await _repository.Create(entity);

            await _teamsUsersRepository.AddTeamUser(entity.Id, userId, UserStatus.SuperAdmin);

            await _invitesService.InviteToTeam(
                entity.Id,
                userId,
                new CreateInvitesDTO
                {
                    Emails = dto.Emails
                });

            var result = _mapper.Map<TeamModel, TeamDTO>(entity);
            return new Response<TeamDTO>
            {
                Data = result
            };
        }
    }
}