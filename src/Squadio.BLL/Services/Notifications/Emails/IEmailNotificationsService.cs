using System.Threading.Tasks;
using Squadio.Common.Models.Emails;
using Squadio.Common.Models.Responses;

namespace Squadio.BLL.Services.Notifications.Emails
{
    public interface IEmailNotificationsService
    {
        Task<Response> SendEmail(MailNotificationModel message);
    }
}