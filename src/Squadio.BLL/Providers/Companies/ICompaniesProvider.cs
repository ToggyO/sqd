using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;

namespace Squadio.BLL.Providers.Companies
{
    public interface ICompaniesProvider
    {
        Task<Response<CompanyDTO>> GetById(Guid id);
    }
}