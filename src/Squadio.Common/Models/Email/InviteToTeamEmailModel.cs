﻿namespace Squadio.Common.Models.Email
{
    public class InviteToTeamEmailModel : EmailAbstractModel
    {
        public string Code { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string AuthorName { get; set; }
    }
}