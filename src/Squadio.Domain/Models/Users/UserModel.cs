using System;
using System.Collections.Generic;
using System.Text;

namespace Squadio.Domain.Models.Users
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}
