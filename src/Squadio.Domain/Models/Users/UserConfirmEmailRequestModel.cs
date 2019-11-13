﻿using System;

namespace Squadio.Domain.Models.Users
{
    public class UserConfirmEmailRequestModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Code { get; set; }
        public bool IsActivated { get; set; }
        public DateTime? ActivatedDate { get; set; }
    }
}