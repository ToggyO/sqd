namespace Squadio.Common.Models.Email
{
    public class InviteToProjectEmailModel : EmailAbstractModel
    {
        public string Code { get; set; }
        public string ProjectName { get; set; }
        public string AuthorName { get; set; }
        public bool IsAlreadyRegistered { get; set; }
    }
}