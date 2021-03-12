using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Projects;
using Squadio.Domain.Models.Users;

namespace Squadio.DAL.Repository.ProjectsUsers
{
    public interface IProjectsUsersRepository
    {
        Task<PageModel<ProjectUserModel>> GetProjectsUsers(PageModel model
            , Guid? userId = null
            , Guid? companyId = null
            , Guid? teamId = null
            , Guid? projectId = null
            , IEnumerable<MembershipStatus> statuses = null);
        Task<IEnumerable<ProjectUserModel>> GetProjectUsersByEmails(Guid projectId, IEnumerable<string> emails);
        Task<ProjectUserModel> GetProjectUser(Guid projectId, Guid userId);
        Task<ProjectUserModel> GetFullProjectUser(Guid projectId, Guid userId);
        Task AddProjectUser(Guid projectId, Guid userId, MembershipStatus membershipStatus);
        Task DeleteProjectUser(Guid projectId, Guid userId);
    }
}