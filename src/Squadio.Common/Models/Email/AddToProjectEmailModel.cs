namespace Squadio.Common.Models.Email
{
    public class AddToProjectEmailModel : EmailAbstractModel
    {
        public string ProjectName { get; set; }
        public string AuthorName { get; set; }
    }
}