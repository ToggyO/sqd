using System;
using Squadio.Domain.Enums;

namespace Squadio.Common.Models.Email
{
    public class InviteUserEmailModel : EmailAbstractModel
    {
        public string Code { get; set; }
        public string EntityName { get; set; }
        public EntityType EntityType { get; set; }
        public string AuthorName { get; set; }
    }
}