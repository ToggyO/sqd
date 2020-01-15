using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Invites;

namespace Squadio.BLL.Services.Companies
{
    public interface ICompaniesService
    {
        Task<Response<CompanyDTO>> Create(Guid userId, CompanyCreateDTO dto);
        Task<Response> InviteUsers(Guid companyId, Guid authorId, CreateInvitesDTO dto);
        Task<Response<CompanyDTO>> Update(Guid companyId, Guid userId, CompanyUpdateDTO dto);
        Task<Response> DeleteUserFromCompany(Guid companyId, Guid removeUserId, Guid currentUserId);
    }
}