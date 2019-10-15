using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;

namespace Squadio.BLL.Services.Companies
{
    public interface ICompaniesService
    {
        Task<Response<CompanyDTO>> Create(Guid userId, CreateCompanyDTO dto);
    }
}