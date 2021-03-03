using System;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;
using Squadio.DTO.Models.Admin;
using Squadio.DTO.Models.Companies;
using Squadio.DTO.Models.Users;

namespace Squadio.BLL.Providers.Admin
{
    public interface IAdminsProvider
    {
        Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage(PageModel model, UserFilterAdminDTO filter);
        Task<Response> SetUserStatus(Guid userId, UserStatus status);
        Task<Response<UserDetailAdminDTO>> GetUserDetail(Guid userId);
        Task<Response<UserDetailAdminDTO>> GetUserDetail(string email);
        Task<Response<PageModel<CompanyDetailAdminDTO>>> GetCompanyPage(PageModel model, CompanyFilterAdminDTO filter);
        Task<Response<CompanyDetailAdminDTO>> GetCompanyDetail(Guid companyId);
    }
}