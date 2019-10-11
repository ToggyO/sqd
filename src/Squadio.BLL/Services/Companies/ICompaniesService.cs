using System;
using System.Threading.Tasks;
using Squadio.DTO.Companies;

namespace Squadio.BLL.Services.Companies
{
    public interface ICompaniesService
    {
        Task<CompanyDTO> Create(Guid userId, CreateCompanyDTO dto);
    }
}