using Squadio.EmailSender.Models.Email;

namespace Squadio.EmailSender.Models.Email
{
    public class PasswordRestoreEmailModel : EmailAbstractModel
    {
        public string Code { get; set; }
    }
}