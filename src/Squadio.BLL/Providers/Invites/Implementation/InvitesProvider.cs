using System;
using System.Collections.Generic;
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
        
        public async Task<Response<InviteModel>> GetInviteByCode(string code)
        {
            var item = await _repository.GetInviteByCode(code);
            if (item == null)
            {
                return new SecurityErrorResponse<InviteModel>(new []
                {
                    new Error
                    {
                        Code = ErrorCodes.Common.NotFound,
                        Message = ErrorMessages.Common.NotFound
                    }
                });
            }

            return new Response<InviteModel>
            {
                Data = item
            };
        }
        
        public async Task<Response<IEnumerable<InviteModel>>> GetInvitesByEntityId(Guid entityId)
        {
            var invites = await _repository.GetInvites(entityId);
            return new Response<IEnumerable<InviteModel>>
            {
                Data = invites
            };
        }
    }
}