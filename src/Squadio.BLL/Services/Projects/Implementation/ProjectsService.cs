using System;
using System.Threading.Tasks;
using Mapper;
using Squadio.BLL.Services.Invites;
using Squadio.Common.Models.Errors;
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

        public async Task<Response<ProjectDTO>> Create(Guid userId, Guid companyId, CreateProjectDTO dto)
        {
            var entity = new ProjectModel
            {
                Name = dto.Name,
                CompanyId = companyId,
                CreatedDate = DateTime.UtcNow,
                ColorHex = dto.ColorHex
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

        public async Task<Response<ProjectDTO>> Update(Guid projectId, Guid userId, ProjectUpdateDTO dto)
        {
            
            var projectEntity = await _repository.GetById(projectId);
            
            if (projectEntity == null)
            {
                return new BusinessConflictErrorResponse<ProjectDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = "projectId"
                    }
                });
            }
            
            var projectUser = await _projectsUsersRepository.GetProjectUser(projectId, userId);
            
            if (projectUser == null)
            {
                return new BusinessConflictErrorResponse<ProjectDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound,
                        Field = "userId"
                    }
                });
            }

            if (projectUser.Status != UserStatus.SuperAdmin)
            {
                return new ForbiddenErrorResponse<ProjectDTO>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Security.Forbidden,
                        Message = ErrorMessages.Security.Forbidden
                    }
                }); 
            }
            
            projectEntity.Name = dto.Name;
            projectEntity.ColorHex = dto.ColorHex;
            
            projectEntity = await _repository.Update(projectEntity);
            
            var result = _mapper.Map<ProjectModel, ProjectDTO>(projectEntity);
            
            return new Response<ProjectDTO>
            {
                Data = result
            };
        }
    }
}