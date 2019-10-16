using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Services.Invites;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.ProjectsUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Teams;
using Squadio.DTO.Projects;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Services.Projects.Implementation
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectsRepository _repository;
        private readonly IProjectsUsersRepository _projectsUsersRepository;
        private readonly IInvitesService _invitesService;
        private readonly IMapper _mapper;

        public ProjectsService(IProjectsRepository repository
            , IProjectsUsersRepository projectsUsersRepository
            , IInvitesService invitesService
            , IMapper mapper)
        {
            _repository = repository;
            _projectsUsersRepository = projectsUsersRepository;
            _invitesService = invitesService;
            _mapper = mapper;
        }

        public async Task<Response<ProjectDTO>> Create(Guid userId, CreateProjectDTO dto)
        {
            var entity = new ProjectModel
            {
                Name = dto.Name,
                CompanyId = dto.CompanyId,
                CreatedDate = DateTime.UtcNow
            };
            
            entity = await _repository.Create(entity);

            await _projectsUsersRepository.AddProjectUser(entity.Id, userId, UserStatus.SuperAdmin);
            
            var result = _mapper.Map<ProjectModel, ProjectDTO>(entity);
            
            try
            {
                if (dto.Emails?.Length > 0)
                {
                    foreach (var email in dto.Emails)
                    {
                        var res = await _invitesService.InviteToProject(
                            "> Придумаю как сюда вставить имя позже <"
                            , entity.Name
                            , entity.Id
                            , email);
                    }
                }
            }
            catch
            {
                // ignored
                // logger here may be
            }

            return new Response<ProjectDTO>
            {
                Data = result
            };
        }
    }
}