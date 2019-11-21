namespace Squadio.EmailSender.Models.Email
{
    public class InviteToCompanyEmailModel : EmailAbstractModel
    {
        public string Code { get; set; }
        public string CompanyName { get; set; }
        public string AuthorName { get; set; }
    }
}