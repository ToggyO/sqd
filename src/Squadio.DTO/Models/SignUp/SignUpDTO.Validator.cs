using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Models.SignUp
{
    public class SignUpDTOValidator: AbstractValidator<SignUpDTO>
    {
        public SignUpDTOValidator()
        {
            RuleFor(model => model.Password)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);

            RuleFor(model => model.Email)
                .EmailAddress().WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}