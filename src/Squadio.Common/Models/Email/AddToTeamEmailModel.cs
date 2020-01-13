namespace Squadio.Common.Models.Email
{
    public class AddToTeamEmailModel : EmailAbstractModel
    {
        public string TeamName { get; set; }
        public string AuthorName { get; set; }
    }
}