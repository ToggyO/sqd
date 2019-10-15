using System.Net;
using System.Threading.Tasks;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.DAL.Repository.Invites;
using Squadio.Domain.Models.Invites;

namespace Squadio.BLL.Providers.Invites.Implementation
{
    public class InvitesProvider : IInvitesProvider
    {
        private readonly IInvitesRepository _repository;

        public InvitesProvider(IInvitesRepository repository)
        {
            _repository = repository;
        }
        
        public async Task<Response<InviteModel>> GetInviteByEmail(string email)
        {
            var item = await _repository.GetInviteByEmail(email);
            if (item == null)
            {
                return new ErrorResponse<InviteModel>
                {
                    Code = ErrorCodes.Security.InviteInvalid,
                    Message = ErrorMessages.Security.InviteInvalid,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }

            return new Response<InviteModel>
            {
                Data = item
            };
        }
    }
}