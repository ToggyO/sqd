using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users
{
    public interface IUsersHandler
    {
        Task<Response<PageModel<UserDTO>>> GetPage(PageModel model);
        Task<Response<UserDTO>> DeleteUser(Guid id);
        Task<Response<UserDTO>> GetById(Guid id);
        Task<Response<UserDTO>> GetCurrentUser(ClaimsPrincipal claims);
        
        
        Task<Response<PageModel<CompanyWithUserRoleDTO>>> GetUserCompanies(Guid id, PageModel model, Guid? companyId = null);
        Task<Response<PageModel<TeamWithUserRoleDTO>>> GetUserTeams(Guid id, PageModel model
            , Guid? companyId = null
            , Guid? teamId = null);
        Task<Response<PageModel<ProjectWithUserRoleDTO>>> GetUserProjects(Guid id, PageModel model
            , Guid? companyId = null
            , Guid? teamId = null
            , Guid? projectId = null);
        
        
        Task<Response<PageModel<CompanyWithUserRoleDTO>>> GetUserCompanies(ClaimsPrincipal claims, PageModel model, Guid? companyId = null);
        Task<Response<PageModel<TeamWithUserRoleDTO>>> GetUserTeams(ClaimsPrincipal claims, PageModel model
            , Guid? companyId = null
            , Guid? teamId = null);
        Task<Response<PageModel<ProjectWithUserRoleDTO>>> GetUserProjects(ClaimsPrincipal claims, PageModel model
            , Guid? companyId = null
            , Guid? teamId = null
            , Guid? projectId = null);
    }
}
