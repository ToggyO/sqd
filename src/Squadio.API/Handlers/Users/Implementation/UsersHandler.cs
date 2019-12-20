using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.BLL.Providers.Companies;
using Squadio.BLL.Providers.Projects;
using Squadio.BLL.Providers.Teams;
using Squadio.BLL.Providers.Users;
using Squadio.BLL.Services.Users;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Filters;
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

        public async Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies(ClaimsPrincipal claims, PageModel model, CompanyFilter filter)
        {
            var result = await _companyProvider.GetUserCompanies(claims.GetUserId(), model);
            return result;
        }

        public async Task<Response<PageModel<TeamUserDTO>>> GetUserTeams(ClaimsPrincipal claims, PageModel model, TeamFilter filter)
        {
            var result = await _teamsProvider.GetUserTeams(claims.GetUserId(), model, filter.CompanyId);
            return result;
        }

        public async Task<Response<PageModel<ProjectUserDTO>>> GetUserProjects(ClaimsPrincipal claims, PageModel model, ProjectFilter filter)
        {
            var result = await _projectsProvider.GetUserProjects(claims.GetUserId(), model, filter.CompanyId, filter.TeamId);
            return result;
        }

        public async Task<Response<UserDTO>> GetById(Guid id)
        {
            var result = await _provider.GetById(id);
            return result;
        }

        public async Task<Response<PageModel<CompanyUserDTO>>> GetUserCompanies(Guid id, PageModel model, CompanyFilter filter)
        {
            var result = await _companyProvider.GetUserCompanies(id, model);
            return result;
        }

        public async Task<Response<PageModel<TeamUserDTO>>> GetUserTeams(Guid id, PageModel model, TeamFilter filter)
        {
            var result = await _teamsProvider.GetUserTeams(id, model, filter.CompanyId);
            return result;
        }

        public async Task<Response<PageModel<ProjectUserDTO>>> GetUserProjects(Guid id, PageModel model, ProjectFilter filter)
        {
            var result = await _projectsProvider.GetUserProjects(id, model, filter.CompanyId, filter.TeamId);
            return result;
        }

        public async Task<Response<UserDTO>> DeleteUser(Guid id)
        {
            var result = await _service.DeleteUser(id);
            return result;
        }
    }
}
