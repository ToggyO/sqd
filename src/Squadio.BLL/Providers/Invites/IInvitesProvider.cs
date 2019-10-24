using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Models.Invites;

namespace Squadio.BLL.Providers.Invites
{
    public interface IInvitesProvider
    {
        Task<Response<InviteModel>> GetInviteByCode(string code);
        Task<Response<IEnumerable<InviteModel>>> GetProjectInvites(Guid projectId);
        Task<Response<IEnumerable<InviteModel>>> GetTeamInvites(Guid teamId);
    }
}