using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.DAL.Repository.Teams;
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
        private readonly IMapper _mapper;

        public TeamsService(ITeamsRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TeamDTO> Create(Guid userId, CreateTeamDTO dto)
        {
            var entityTeam = new TeamModel
            {
                Name = dto.Name,
                CompanyId = dto.CompanyId,
                CreatedDate = DateTime.UtcNow
            };
            
            entityTeam = await _repository.Create(entityTeam);

            // TODO: add members to teams
            //await _companiesUsersRepository.AddCompanyUser(entityCompany.Id, userId, UserStatus.SuperAdmin);
            
            var result = _mapper.Map<TeamModel, TeamDTO>(entityTeam);
            return result;
        }
    }
}