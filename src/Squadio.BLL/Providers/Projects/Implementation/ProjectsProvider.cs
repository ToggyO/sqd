﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapper;
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

        public async Task<Response<PageModel<UserDTO>>> GetProjectUsers(Guid projectId, PageModel model)
        {
            var page = await _projectsUsersRepository.GetProjectUsers(projectId, model);

            var result = new PageModel<UserDTO>
            {
                Page = page.Page,
                PageSize = page.PageSize,
                Total = page.Total,
                Items = _mapper.Map<IEnumerable<UserModel>,IEnumerable<UserDTO>>(page.Items)
            };
            
            return new Response<PageModel<UserDTO>>
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