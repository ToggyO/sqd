using System;
using System.Collections.Generic;
using System.Text;

namespace Squadio.Domain.Models.Users
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}
