using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Users
{
    public class ChangeEmailRequestDTOValidator: AbstractValidator<ChangeEmailRequestDTO>
    {
        public ChangeEmailRequestDTOValidator()
        {
            RuleFor(model => model.NewEmail)
                .EmailAddress().WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}