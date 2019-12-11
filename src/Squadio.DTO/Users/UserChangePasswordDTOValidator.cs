﻿using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Users
{
    public class UserChangePasswordDTOValidator: AbstractValidator<UserChangePasswordDTO>
    {
        public UserChangePasswordDTOValidator()
        {
            RuleFor(model => model.NewPassword)
                .MinimumLength(6).WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}