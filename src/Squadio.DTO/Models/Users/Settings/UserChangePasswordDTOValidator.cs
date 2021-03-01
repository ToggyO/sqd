using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Models.Users.Settings
{
    public class UserChangePasswordDTOValidator: AbstractValidator<UserSetPasswordDTO>
    {
        public UserChangePasswordDTOValidator()
        {
            RuleFor(model => model.Password)
                .MinimumLength(6).WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}