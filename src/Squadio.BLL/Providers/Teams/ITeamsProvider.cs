using System;
using System.Threading.Tasks;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Providers.Teams
{
    public interface ITeamsProvider
    {
        Task<TeamDTO> GetById(Guid id);
    }
}