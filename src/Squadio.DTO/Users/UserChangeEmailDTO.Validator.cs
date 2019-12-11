using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Users
{
    public class UserChangeEmailRequestDTOValidator: AbstractValidator<UserChangeEmailRequestDTO>
    {
        public UserChangeEmailRequestDTOValidator()
        {
            RuleFor(model => model.NewEmail)
                .EmailAddress().WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}