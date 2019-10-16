using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Companies;
using Squadio.DTO.Pages;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Companies
{
    public interface ICompaniesProvider
    {
        Task<Response<PageModel<UserDTO>>> GetCompanyUsers(Guid companyId, PageModel model);
        Task<Response<CompanyDTO>> GetById(Guid id);
    }
}