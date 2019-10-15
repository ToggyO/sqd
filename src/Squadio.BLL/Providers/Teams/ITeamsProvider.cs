using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Teams;

namespace Squadio.BLL.Providers.Teams
{
    public interface ITeamsProvider
    {
        Task<Response<TeamDTO>> GetById(Guid id);
    }
}