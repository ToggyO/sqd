using FluentValidation;
using Squadio.Common.Models.Errors;
using Squadio.DTO.Users;

namespace Squadio.DTO.SignUp
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