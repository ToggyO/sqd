using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Users.Settings
{
    public class UserSetPasswordDTOValidator: AbstractValidator<UserResetPasswordDTO>
    {
        public UserSetPasswordDTOValidator()
        {
            RuleFor(model => model.Code)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            
            RuleFor(model => model.Password)
                .MinimumLength(6).WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}