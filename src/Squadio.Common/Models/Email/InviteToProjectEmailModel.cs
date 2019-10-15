namespace Squadio.Common.Models.Email
{
    public class InviteToProjectEmailModel : EmailAbstractModel
    {
        public string Code { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string AuthorName { get; set; }
    }
}