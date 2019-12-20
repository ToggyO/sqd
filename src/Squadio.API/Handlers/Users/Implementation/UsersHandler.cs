using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Providers.Teams;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Users;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Projects;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.API.Handlers.Users.Implementation
{
    public class UsersHandler : IUsersHandler
    {
        private readonly IUsersProvider _provider;
        private readonly ICompaniesProvider _companyProvider;
        private readonly ITeamsProvider _teamsProvider;
        private readonly IProjectsProvider _projectsProvider;
        private readonly IUsersService _service;
        public UsersHandler(IUsersProvider provider
            , ICompaniesProvider companyProvider
            , ITeamsProvider teamsProvider
            , IProjectsProvider projectsProvider
            , IUsersService service)
        {
            _service = service;
            _companyProvider = companyProvider;
            _teamsProvider = teamsProvider;
            _projectsProvider = projectsProvider;
            _provider = provider;
        }

        public async Task<Response<PageModel<UserDTO>>> GetPage(PageModel model)
        {
            var result = await _provider.GetPage(model);
            return result;
        }

        public async Task<Response<UserDTO>> GetCurrentUser(ClaimsPrincipal claims)
        {
            var result = await _provider.GetById(claims.GetUserId());
            return result;
        }

        public async Task<Response<PageModel<CompanyWithUserRoleDTO>>> GetUserCompanies(ClaimsPrincipal claims, PageModel model, Guid? companyId = null)
        {
            var result = await _companyProvider.GetUserCompanies(claims.GetUserId(), model, companyId);
            return result;
        }

        public async Task<Response<PageModel<TeamWithUserRoleDTO>>> GetUserTeams(ClaimsPrincipal claims, PageModel model
            , Guid? companyId = null
            , Guid? teamId = null)
        {
            var result = await _teamsProvider.GetUserTeams(claims.GetUserId(), model, companyId, teamId);
            return result;
        }

        public async Task<Response<PageModel<ProjectWithUserRoleDTO>>> GetUserProjects(ClaimsPrincipal claims, PageModel model
            , Guid? companyId = null
            , Guid? teamId = null
            , Guid? projectId = null)
        {
            var result = await _projectsProvider.GetUserProjects(claims.GetUserId(), model, companyId, teamId, projectId);
            return result;
        }

        public async Task<Response<UserDTO>> GetById(Guid id)
        {
            var result = await _provider.GetById(id);
            return result;
        }

        public async Task<Response<PageModel<CompanyWithUserRoleDTO>>> GetUserCompanies(Guid id, PageModel model, Guid? companyId = null)
        {
            var result = await _companyProvider.GetUserCompanies(id, model, companyId);
            return result;
        }

        public async Task<Response<PageModel<TeamWithUserRoleDTO>>> GetUserTeams(Guid id, PageModel model
            , Guid? companyId = null
            , Guid? teamId = null)
        {
            var result = await _teamsProvider.GetUserTeams(id, model, companyId, teamId);
            return result;
        }

        public async Task<Response<PageModel<ProjectWithUserRoleDTO>>> GetUserProjects(Guid id, PageModel model
            , Guid? companyId = null
            , Guid? teamId = null
            , Guid? projectId = null)
        {
            var result = await _projectsProvider.GetUserProjects(id, model, companyId, teamId, projectId);
            return result;
        }

        public async Task<Response<UserDTO>> DeleteUser(Guid id)
        {
            var result = await _service.DeleteUser(id);
            return result;
        }
    }
}
