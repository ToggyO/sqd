using System.Threading.Tasks;
using Squadio.Common.Models.Emails;
using Squadio.Common.Models.Responses;
using Squadio.DTO.Models.Notifications;

namespace Squadio.BLL.Services.Notifications.Emails
{
    public interface IEmailNotificationsService
    {
        Task<Response> SendEmail(MailNotificationModel message);
        Task<Response> SendEmail(EmailMessageDTO message);
        Task<Response> SendResetPasswordEmail(string email, string code);
    }
}