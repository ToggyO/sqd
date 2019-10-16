using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Models.Teams;
using Squadio.DTO.Pages;

namespace Squadio.DAL.Repository.Teams
{
    public interface ITeamsRepository : IBaseRepository<TeamModel>
    {
        Task<PageModel<TeamModel>> GetTeams(PageModel model);
    }
}