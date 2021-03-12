using System.Threading.Tasks;
using Squadio.Common.Models.Emails;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Notifications;

namespace Squadio.BLL.Services.Notifications.Emails
{
    public interface IEmailNotificationsService
    {
        Task<Response> SendEmail(EmailMessageDTO message);
        Task<Response> SendResetAdminPasswordEmail(string email, string code);
        Task<Response> SendConfirmNewAdminMailboxEmail(string newEmail, string code);
        Task<Response> SendResetPasswordEmail(string email, string code);
        Task<Response> SendConfirmNewMailboxEmail(string newEmail, string code);
        Task<Response> SendInviteEmail(string email, string authorName, string entityName, string entityType, string code);
    }
}