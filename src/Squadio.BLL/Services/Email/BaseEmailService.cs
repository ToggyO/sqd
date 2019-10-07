using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Squadio.BLL.Services.Email.Sender;
using Squadio.Common.Models.Email;
using Squadio.Common.Settings;

namespace Squadio.BLL.Services.Email
{
    public abstract class BaseEmailService<TEmailModel> : IEmailService<TEmailModel>
        where TEmailModel : EmailAbstractModel
    {
        protected readonly IOptions<EmailSettingsModel> _emailSettings;
        private readonly IEmailSender _emailSender;

        protected BaseEmailService(IOptions<EmailSettingsModel> emailSettings, IEmailSender emailSender)
        {
            _emailSettings = emailSettings;
            _emailSender = emailSender;
        }

        public virtual async Task Send(TEmailModel emailModel)
        {
            await _emailSender.SendAsync(emailModel.To, GetSubject(emailModel), GetHtmlTemplate(emailModel));
        }

        protected abstract string GetHtmlTemplate(TEmailModel model);

        protected virtual string GetSubject(TEmailModel emailModel)
        {
            return "";
        }
    }
}