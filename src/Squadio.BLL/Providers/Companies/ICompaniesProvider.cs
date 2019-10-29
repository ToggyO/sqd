using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Companies;
using Squadio.DTO.Pages;
using Squadio.DTO.Users;

namespace Squadio.BLL.Providers.Companies
{
    public interface ICompaniesProvider
    {
        Task<Response<PageModel<CompanyDTO>>> GetCompaniesOfUser(Guid userId, PageModel model, UserStatus? status = null);
        Task<Response<PageModel<UserDTO>>> GetCompanyUsers(Guid companyId, PageModel model);
        Task<Response<CompanyDTO>> GetById(Guid id);
    }
}