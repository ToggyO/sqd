using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Models.Invites;

namespace Squadio.BLL.Providers.Invites
{
    public interface IInvitesProvider
    {
        Task<Response<InviteModel>> GetInviteByEmail(string email);
    }
}