using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Squadio.Common.Models.Pages;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Admin;
using Squadio.DTO.Models.Auth;
using Squadio.DTO.Models.Users;
using Squadio.DTO.Models.Users.Settings;

namespace Squadio.API.Handlers.Admins
{
    public interface IAdminsHandler
    {
        Task<Response<PageModel<UserWithCompaniesDTO>>> GetUsersPage(PageModel model, UserFilterAdminDTO filter);
        Task<Response<UserDetailAdminDTO>> GetUserDetail(Guid userId);
        Task<Response> BlockUser(Guid userId);
        Task<Response> UnblockUser(Guid userId);
        Task<Response> ChangePassword(UserSetPasswordDTO dto, ClaimsPrincipal claims);
        Task<Response> ResetPasswordRequest(string email);
        Task<Response> ResetPassword(UserResetPasswordDTO dto);
        Task<Response<AuthInfoDTO>> Authenticate(CredentialsDTO request);
        Task<Response<TokenDTO>> RefreshToken(string refreshToken);
        Task<Response<PageModel<CompanyDetailAdminDTO>>> GetCompanyPage(PageModel model, CompanyFilterAdminDTO filter);
        Task<Response<CompanyDetailAdminDTO>> GetCompanyDetail(Guid companyId);
        Task<Response<PageModel<UserWithRoleDTO>>> GetCompanyUsersPage(PageModel model, Guid companyId);
        Task<Response> BlockCompany(Guid companyId);
        Task<Response> UnblockCompany(Guid companyId);
    }
}