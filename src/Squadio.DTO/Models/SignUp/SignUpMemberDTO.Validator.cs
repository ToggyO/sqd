using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Models.SignUp
{
    public class SignUpMemberDTOValidator: AbstractValidator<SignUpMemberDTO>
    {
        public SignUpMemberDTOValidator()
        {
            RuleFor(model => model.Password)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
            
            RuleFor(model => model.InviteCode)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}