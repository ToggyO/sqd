using System;
using System.ComponentModel.DataAnnotations;

namespace Squadio.DTO.Teams
{
    public class CreateTeamDTO
    {
        public string Name { get; set; }
        public string[] Emails { get; set; }
        public string ColorHex { get; set; }
    }
}