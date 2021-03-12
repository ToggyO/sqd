using System;
using System.Threading.Tasks;
using AutoMapper;
using Squadio.BLL.Services.Membership;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.CompaniesUsers;
using Squadio.DAL.Repository.Projects;
using Squadio.DAL.Repository.ProjectsUsers;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Projects;
using Squadio.DTO.Models.Invites;
using Squadio.DTO.Models.Projects;
using Squadio.DTO.Projects;

namespace Squadio.BLL.Services.Projects.Implementations
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectsRepository _repository;
        private readonly IProjectsUsersRepository _projectsUsersRepository;
        private readonly ICompaniesUsersRepository _companiesUsersRepository;
        private readonly IMembershipService _membershipService;
        private readonly IMapper _mapper;

        public ProjectsService(IProjectsRepository repository
            , IProjectsUsersRepository projectsUsersRepository
            , ICompaniesUsersRepository companiesUsersRepository
            , IMembershipService membershipService
            , IMapper mapper)
        {
            _repository = repository;
            _projectsUsersRepository = projectsUsersRepository;
            _companiesUsersRepository = companiesUsersRepository;
            _membershipService = membershipService;
            _mapper = mapper;
        }

        public async Task<Response<ProjectDTO>> Create(Guid userId, Guid teamId, ProjectCreateDTO dto, bool sendInvites = true)
        {
            var entity = new ProjectModel
            {
                Name = dto.Name,
                TeamId = teamId,
                CreatedDate = DateTime.UtcNow,
                ColorHex = dto.ColorHex,
                CreatorId = userId
            };
            
            entity = await _repository.Create(entity);

            await _projectsUsersRepository.AddProjectUser(entity.Id, userId, MembershipStatus.SuperAdmin);
            
            //TODO:
            // await _membershipService.InviteUsersToProject(entity.Id, userId, new CreateInvitesDTO {Emails = dto.Emails}, sendInvites);
            
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

            if (projectUser.Status != MembershipStatus.SuperAdmin)
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

        public async Task<Response<ProjectDTO>> Delete(Guid projectId, Guid userId)
        {
            var projectUser = await _projectsUsersRepository.GetFullProjectUser(projectId, userId);
            if (projectUser == null || projectUser.Status != MembershipStatus.SuperAdmin)
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

            var companyUser = await _companiesUsersRepository.GetCompanyUser(projectUser.Project.Team.CompanyId, userId);
            if (companyUser == null || companyUser.Status != MembershipStatus.SuperAdmin)
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

            var entity = await _repository.Delete(projectId);
            return new Response<ProjectDTO>
            {
                Data = _mapper.Map<ProjectModel, ProjectDTO>(entity)
            };
        }

    }
}