using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Filters;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Models.Teams;

namespace Squadio.DAL.Repository.Teams
{
    public interface ITeamsRepository : IBaseRepository<TeamModel>
    {
        Task<PageModel<TeamModel>> GetTeams(PageModel model, Guid? companyId = null);
    }
}