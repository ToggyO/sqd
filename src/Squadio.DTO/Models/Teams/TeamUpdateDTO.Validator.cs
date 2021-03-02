using FluentValidation;
using Squadio.Common.Models.Errors;

namespace Squadio.DTO.Models.Teams
{
    public class TeamUpdateDTOValidator: AbstractValidator<TeamUpdateDTO>
    {
        public TeamUpdateDTOValidator()
        {
            RuleFor(model => model.ColorHex)
                .NotEmpty()
                .Length(7)
                .Must((model) =>
                {
                    if(string.IsNullOrEmpty(model))
                        return false;
                    
                    return model[0] == '#';
                }).WithErrorCode(ErrorCodes.Common.FieldInvalid);

            RuleFor(model => model.Name)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}