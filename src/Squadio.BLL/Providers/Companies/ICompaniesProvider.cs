using System;
using System.Threading.Tasks;
using Squadio.DTO.Companies;

namespace Squadio.BLL.Providers.Companies
{
    public interface ICompaniesProvider
    {
        Task<CompanyDTO> GetById(Guid id);
    }
}