using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.ProjectsUsers;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Users;
using Squadio.DTO.Projects;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Projects.Implementation
{
    public class ProjectsProvider : IProjectsProvider
    {
        private readonly IProjectsRepository _repository;
        private readonly IProjectsUsersRepository _projectsUsersRepository;
        private readonly IMapper _mapper;

        public ProjectsProvider(IProjectsRepository repository
            , IProjectsUsersRepository projectsUsersRepository
            , IMapper mapper)
        {
            _repository = repository;
            _projectsUsersRepository = projectsUsersRepository;
            _mapper = mapper;
        }

        public async Task<Response<PageModel<ProjectDTO>>> GetProjects(PageModel model, Guid? companyId = null, Guid? teamId = null)
        {
            var page = await _repository.GetProjects(model, companyId, teamId);

            var result = new PageModel<ProjectDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = _mapper.Map<IEnumerable<ProjectModel>,IEnumerable<ProjectDTO>>(page.Items)
            };
            
            return new Response<PageModel<ProjectDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<PageModel<ProjectWithUserRoleDTO>>> GetUserProjects(Guid userId, PageModel model
            , Guid? companyId = null
            , Guid? teamId = null
            , Guid? projectId = null)
        {
            var page = await _projectsUsersRepository.GetProjectsUsers(model, userId, companyId, teamId, projectId);
            
            var items = page.Items.Select(x => _mapper.Map<ProjectUserModel, ProjectWithUserRoleDTO>(x)).ToList();

            var result = new PageModel<ProjectWithUserRoleDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = items
            };
            
            return new Response<PageModel<ProjectWithUserRoleDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<PageModel<UserWithRoleDTO>>> GetProjectUsers(Guid projectId, PageModel model)
        {
            var page = await _projectsUsersRepository.GetProjectsUsers(model, projectId: projectId);
            
            var items = page.Items.Select(x => _mapper.Map<ProjectUserModel, UserWithRoleDTO>(x)).ToList();

            var result = new PageModel<UserWithRoleDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = items
            };
            
            return new Response<PageModel<UserWithRoleDTO>>
            {
                Data = result
            };
        }

        public async Task<Response<ProjectDTO>> GetById(Guid id)
        {
            
            var entity = await _repository.GetById(id);

            var result = _mapper.Map<ProjectModel, ProjectDTO>(entity);

            return new Response<ProjectDTO>
            {
                Data = result
            };
        }
    }
}