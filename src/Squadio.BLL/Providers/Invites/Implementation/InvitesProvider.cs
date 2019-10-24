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
                        Code = ErrorCodes.Security.InviteInvalid,
                        Message = ErrorMessages.Security.InviteInvalid
                    }
                });
            }

            return new Response<InviteModel>
            {
                Data = item
            };
        }

        public async Task<Response<IEnumerable<InviteModel>>> GetProjectInvites(Guid projectId)
        {
            var invites = await _repository.GetProjectInvites(projectId);
            return new Response<IEnumerable<InviteModel>>
            {
                Data = invites
            };
        }

        public async Task<Response<IEnumerable<InviteModel>>> GetTeamInvites(Guid teamId)
        {
            var invites = await _repository.GetTeamInvites(teamId);
            return new Response<IEnumerable<InviteModel>>
            {
                Data = invites
            };
        }
    }
}