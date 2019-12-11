using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Users
{
    public class UserUpdateDTOValidator: AbstractValidator<UserUpdateDTO>
    {
        public UserUpdateDTOValidator()
        {
            RuleFor(model => model.Name)
                .NotEmpty()
                .MaximumLength(60)
                .WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}