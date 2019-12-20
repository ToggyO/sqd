using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Users.Settings
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