using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Users
{
    public class UserSetPasswordDTOValidator: AbstractValidator<UserResetPasswordDTO>
    {
        public UserSetPasswordDTOValidator()
        {
            RuleFor(model => model.Code)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            
            RuleFor(model => model.Password)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}