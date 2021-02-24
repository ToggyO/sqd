using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Models.Auth
{
    public class CredentialsDTOValidator: AbstractValidator<CredentialsDTO>
    {
        public CredentialsDTOValidator()
        {
            RuleFor(model => model.Email)
                .NotEmpty().WithErrorCode(ErrorCodes.Security.AuthDataInvalid)
                .EmailAddress().WithErrorCode(ErrorCodes.Security.AuthDataInvalid);
            
            RuleFor(model => model.Password)
                .NotEmpty().WithErrorCode(ErrorCodes.Security.AuthDataInvalid);
        }
    }
}