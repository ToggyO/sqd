using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Companies
{
    public interface ICompanyInvitesService
    {
        Task<Response> CreateInvite(Guid companyId, Guid authorId, CreateInvitesDTO dto, bool sendInvites = true);
        Task<Response> CancelInvite(Guid companyId, Guid authorId, CancelInvitesDTO dto);
        Task<Response> AcceptInvite(Guid userId, string code);
        Task<Response> AcceptInvite(Guid companyId, Guid userId);
    }
}