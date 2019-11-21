namespace Squadio.EmailSender.Models.Email
{
    public class UserConfirmEmailModel : EmailAbstractModel
    {
        public string Code { get; set; }
    }
}