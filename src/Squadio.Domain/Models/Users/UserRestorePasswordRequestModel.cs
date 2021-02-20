using System;

namespace Squadio.Domain.Models.Users
{
    public class UserRestorePasswordRequestModel : BaseModel
    {
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
        public string Code { get; set; }
    }
}