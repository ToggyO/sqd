using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Invites;
using Squadio.DAL.Repository.ProjectsUsers;
using Squadio.DAL.Repository.TeamsUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Invites;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Providers.Invites.Implementation
{
    public class InvitesProvider : IInvitesProvider
    {
        private readonly IInvitesRepository _repository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly ITeamsUsersRepository _teamsUsersRepository;
        private readonly IProjectsUsersRepository _projectsUsersRepository;
        private readonly IMapper _mapper;

        public InvitesProvider(IInvitesRepository repository
            , ICompaniesUsersRepository companiesUsersRepository
            , ITeamsUsersRepository teamsUsersRepository
            , IProjectsUsersRepository projectsUsersRepository
            , IMapper mapper)
        {
            _repository = repository;
            _companiesUsersRepository = companiesUsersRepository;
            _teamsUsersRepository = teamsUsersRepository;
            _projectsUsersRepository = projectsUsersRepository;
            _mapper = mapper;
        }

        public async Task<Response<InviteModel>> GetInviteByCode(string code)
        {
            var item = await _repository.GetInviteByCode(code);
            if (item == null)
            {
                return new SecurityErrorResponse<InviteModel>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound
                    }
                });
            }

            return new Response<InviteModel>
            {
                Data = item
            };
        }
        
        public async Task<Response<IEnumerable<InviteDTO>>> GetInvitesByEntityId(Guid entityId, Guid userId, EntityType entityType, bool? activated = null)
        {
            switch (entityType)
            {
                case EntityType.Company:
                    var companyUser = await _companiesUsersRepository.GetCompanyUser(entityId, userId);
                    if (companyUser == null || companyUser?.Status == UserStatus.Member)
                    {
                        return new ForbiddenErrorResponse<IEnumerable<InviteDTO>>(new []
                        {
                            new Error
                            {
                                Code = ErrorCodes.Security.Forbidden,
                                Message = ErrorMessages.Security.Forbidden,
                            }
                        });
                    }
                    break;
                case EntityType.Team:
                    var teamUser = await _teamsUsersRepository.GetTeamUser(entityId, userId);
                    if (teamUser == null || teamUser?.Status == UserStatus.Member)
                    {
                        return new ForbiddenErrorResponse<IEnumerable<InviteDTO>>(new []
                        {
                            new Error
                            {
                                Code = ErrorCodes.Security.Forbidden,
                                Message = ErrorMessages.Security.Forbidden,
                            }
                        });
                    }
                    break;
                case EntityType.Project:
                    var projectUser = await _projectsUsersRepository.GetProjectUser(entityId, userId);
                    if (projectUser == null || projectUser?.Status == UserStatus.Member)
                    {
                        return new ForbiddenErrorResponse<IEnumerable<InviteDTO>>(new []
                        {
                            new Error
                            {
                                Code = ErrorCodes.Security.Forbidden,
                                Message = ErrorMessages.Security.Forbidden,
                            }
                        });
                    }
                    break;
            }

            return await GetInvitesByEntityId(entityId, activated);
        }
        
        public async Task<Response<IEnumerable<InviteDTO>>> GetInvitesByEntityId(Guid entityId, bool? activated = null)
        {
            var invites = await _repository.GetInvites(
                entityId: entityId, 
                activated: activated);
            return new Response<IEnumerable<InviteDTO>>
            {
                Data = invites.Select(x => _mapper.Map<InviteModel, InviteDTO>(x))
            };
        }
    }
}