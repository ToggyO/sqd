﻿using Mapper;
using Squadio.DAL.Repository.Teams;

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
    }
}