namespace Squadio.Common.Models.Email
{
    public class PasswordResetEmailModel : EmailAbstractModel
    {
        public string Code { get; set; }
    }
}