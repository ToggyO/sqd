using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Pages;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Providers.Teams
{
    public interface ITeamsProvider
    {
        Task<Response<PageModel<TeamDTO>>> GetTeams(PageModel model);
        Task<Response<TeamDTO>> GetById(Guid id);
    }
}