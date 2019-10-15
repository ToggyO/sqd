using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.ProjectsUsers;
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
        private readonly IMapper _mapper;

        public ProjectsService(IProjectsRepository repository
            , IProjectsUsersRepository projectsUsersRepository
            , IMapper mapper)
        {
            _repository = repository;
            _projectsUsersRepository = projectsUsersRepository;
            _mapper = mapper;
        }

        public async Task<ProjectDTO> Create(Guid userId, CreateProjectDTO dto)
        {
            var entity = new ProjectModel
            {
                Name = dto.Name,
                CompanyId = dto.CompanyId,
                CreatedDate = DateTime.UtcNow
            };
            
            entity = await _repository.Create(entity);

            // TODO: send to members invites to project across email
            // p.s. NOT ADD IMMEDIATELY to projects.
            
            var result = _mapper.Map<ProjectModel, ProjectDTO>(entity);
            return result;
        }
    }
}