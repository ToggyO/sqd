using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Teams;
using Squadio.Domain.Models.Teams;
using Squadio.DTO.Pages;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Providers.Teams.Implementation
{
    public class TeamsProvider : ITeamsProvider
    {
        private readonly ITeamsRepository _repository;
        private readonly IMapper _mapper;

        public TeamsProvider(ITeamsRepository repository
            , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<PageModel<TeamDTO>>> GetTeams(PageModel model)
        {
            var page = await _repository.GetTeams(model);

            var result = new PageModel<TeamDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = _mapper.Map<IEnumerable<TeamModel>,IEnumerable<TeamDTO>>(page.Items)
            };
            
            return new Response<PageModel<TeamDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<TeamDTO>> GetById(Guid id)
        {
            var entity = await _repository.GetById(id);

            var result = _mapper.Map<TeamModel, TeamDTO>(entity);

            return new Response<TeamDTO>
            {
                Data = result
            };
        }
    }
}