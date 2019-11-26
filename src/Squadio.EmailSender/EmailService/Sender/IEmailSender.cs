using System.Threading.Tasks;
using Squadio.Common.Models.Email;

namespace Squadio.EmailSender.EmailService.Sender
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string content, EmailAttachment[] attachmentItems = null, string styles = "");
    }
}