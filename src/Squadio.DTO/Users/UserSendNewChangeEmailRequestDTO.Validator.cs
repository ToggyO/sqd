using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Users
{
    public class UserSendNewChangeEmailRequestDTOValidator: AbstractValidator<UserSendNewChangeEmailRequestDTO>
    {
        public UserSendNewChangeEmailRequestDTOValidator()
        {
            RuleFor(model => model.NewEmail)
                .EmailAddress().WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}