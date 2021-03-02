using FluentValidation;
using Squadio.Common.Models.Errors;
using Squadio.DTO.Models.Projects;

namespace Squadio.DTO.Projects
{
    public class ProjectCreateDTOValidator: AbstractValidator<ProjectCreateDTO>
    {
        public ProjectCreateDTOValidator()
        {
            RuleFor(model => model.ColorHex)
                .NotEmpty()
                .Length(7)
                .Must((model) =>
                {
                    if(string.IsNullOrEmpty(model))
                        return false;
                    
                    return model[0] == '#';
                })
                .WithMessage(ErrorMessages.Common.FieldInvalid)
                .WithErrorCode(ErrorCodes.Common.FieldInvalid);

            RuleFor(model => model.Name)
                .NotEmpty().WithErrorCode(ErrorCodes.Common.FieldInvalid);
        }
    }
}