﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Teams;
using Squadio.DAL.Repository.TeamsUsers;
using Squadio.Domain.Models.Teams;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Teams.Implementation
{
    public class TeamsProvider : ITeamsProvider
    {
        private readonly ITeamsRepository _repository;
        private readonly ITeamsUsersRepository _teamsUsersRepository;
        private readonly IMapper _mapper;

        public TeamsProvider(ITeamsRepository repository
            , ITeamsUsersRepository teamsUsersRepository
            , IMapper mapper)
        {
            _repository = repository;
            _teamsUsersRepository = teamsUsersRepository;
            _mapper = mapper;
        }

        public async Task<Response<PageModel<TeamUserDTO>>> GetUserTeams(Guid userId, PageModel model, Guid? companyId = null)
        {
            var page = await _teamsUsersRepository.GetUserTeams(userId, model, companyId);
            
            var items = page.Items.Select(x => _mapper.Map<TeamUserModel, TeamUserDTO>(x)).ToList();
            items.ForEach(x => x.User = null);

            var result = new PageModel<TeamUserDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = items
            };
            
            return new Response<PageModel<TeamUserDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<PageModel<TeamUserDTO>>> GetTeamUsers(Guid teamId, PageModel model)
        {
            var page = await _teamsUsersRepository.GetTeamUsers(teamId, model);
            
            var items = page.Items.Select(x => _mapper.Map<TeamUserModel, TeamUserDTO>(x)).ToList();
            items.ForEach(x => x.Team = null);

            var result = new PageModel<TeamUserDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = items
            };
            
            return new Response<PageModel<TeamUserDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<PageModel<TeamDTO>>> GetTeams(PageModel model, TeamFilter filter)
        {
            var page = await _repository.GetTeams(model, filter);

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