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
using Squadio.DTO.Invites;
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
            
            await _invitesService.InviteToProject(
                entity.Id,
                userId,
                new CreateInvitesDTO
                {
                    Emails = dto.Emails
                });
            
            var result = _mapper.Map<ProjectModel, ProjectDTO>(entity);
            return new Response<ProjectDTO>
            {
                Data = result
            };
        }
    }
}