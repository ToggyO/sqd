using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Teams;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Teams
{
    public interface ITeamsProvider
    {
        Task<Response<PageModel<TeamDTO>>> GetTeams(PageModel model, TeamFilter filter);
        Task<Response<PageModel<TeamWithUserRoleDTO>>> GetUserTeams(Guid userId, PageModel model, Guid? companyId = null);
        Task<Response<PageModel<UserWithRoleDTO>>> GetTeamUsers(Guid teamId, PageModel model);
        Task<Response<TeamDTO>> GetById(Guid id);
    }
}