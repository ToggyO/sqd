using Squadio.Common.Enums;

namespace Squadio.Common.Models.Email
{
    public class AddUserEmailModel : EmailAbstractModel
    {
        public string EntityName { get; set; }
        public EntityType EntityType { get; set; }
        public string AuthorName { get; set; }
    }
}