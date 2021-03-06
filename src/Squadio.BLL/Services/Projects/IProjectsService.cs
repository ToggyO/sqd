using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Projects;
using Squadio.DTO.Projects;

namespace Squadio.BLL.Services.Projects
{
    public interface IProjectsService
    {
        Task<Response<ProjectDTO>> Create(Guid userId, Guid teamId, ProjectCreateDTO dto, bool sendInvites = true);
        Task<Response<ProjectDTO>> Update(Guid projectId, Guid userId, ProjectUpdateDTO dto);
        Task<Response<ProjectDTO>> Delete(Guid projectId, Guid userId);
        //Task<Response> InviteUsers(Guid projectId, Guid authorId, CreateInvitesDTO dto, bool sendMails = true);
        //Task<Response> DeleteUserFromProject(Guid projectId, Guid removeUserId, Guid currentUserId);
        //Task<Response> DeleteUserFromProjectsByTeamId(Guid teamId, Guid removeUserId);
    }
}