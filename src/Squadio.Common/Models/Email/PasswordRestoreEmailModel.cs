namespace Squadio.Common.Models.Email
{
    public class PasswordRestoreEmailModel : EmailAbstractModel
    {
        public string Code { get; set; }
    }
}