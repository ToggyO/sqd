namespace Squadio.Common.Models.Email
{
    public class PasswordRestoreAdminEmailModel : EmailAbstractModel
    {
        public string Code { get; set; }
    }
}