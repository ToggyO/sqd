namespace Squadio.Common.Models.Email
{
    public class AddToCompanyEmailModel : EmailAbstractModel
    {
        public string CompanyName { get; set; }
        public string AuthorName { get; set; }
    }
}