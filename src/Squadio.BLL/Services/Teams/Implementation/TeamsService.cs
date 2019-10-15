using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.TeamsUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Companies;
using Squadio.Domain.Models.Teams;
using Squadio.DTO.Companies;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Services.Teams.Implementation
{
    public class TeamsService : ITeamsService
    {
        private readonly ITeamsRepository _repository;
        private readonly ITeamsUsersRepository _teamsUsersRepository;
        private readonly IMapper _mapper;

        public TeamsService(ITeamsRepository repository
            , ITeamsUsersRepository teamsUsersRepository
            , IMapper mapper)
        {
            _repository = repository;
            _teamsUsersRepository = teamsUsersRepository;
            _mapper = mapper;
        }

        public async Task<Response<TeamDTO>> Create(Guid userId, CreateTeamDTO dto)
        {
            var entityTeam = new TeamModel
            {
                Name = dto.Name,
                CompanyId = dto.CompanyId,
                CreatedDate = DateTime.UtcNow
            };
            
            entityTeam = await _repository.Create(entityTeam);

            // TODO: send to members invites to team across email
            // p.s. NOT ADD IMMEDIATELY to team.
            
            var result = _mapper.Map<TeamModel, TeamDTO>(entityTeam);
            return new Response<TeamDTO>
            {
                Data = result
            };
        }
    }
}