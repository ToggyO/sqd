using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;

namespace Squadio.BLL.Services.Companies
{
    public interface ICompaniesService
    {
        Task<Response<CompanyDTO>> Create(Guid userId, CreateCompanyDTO dto);
        Task<Response<CompanyDTO>> Update(Guid companyId, Guid userId, CompanyUpdateDTO dto);
        Task<Response> DeleteUserFromCompany(Guid companyId, Guid removeUserId, Guid currentUserId);
    }
}