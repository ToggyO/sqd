using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Models.Users.Settings
{
    public class UserResetPasswordDTOValidator: AbstractValidator<UserResetPasswordDTO>
    {
        public UserResetPasswordDTOValidator()
        {
            RuleFor(model => model.Code)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            
            RuleFor(model => model.Password)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid)
                .MinimumLength(6).WithErrorCode(ErrorCodes.Common.FieldInvalidLength);
        }
    }
}