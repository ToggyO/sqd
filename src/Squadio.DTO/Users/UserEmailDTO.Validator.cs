﻿using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Users
{
    public class UserEmailDTOValidator: AbstractValidator<UserEmailDTO>
    {
        public UserEmailDTOValidator()
        {
            RuleFor(model => model.Email)
                .EmailAddress().WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}