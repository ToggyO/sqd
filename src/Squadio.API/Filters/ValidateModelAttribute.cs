using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Squadio.Common.Models.Responses;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Squadio.API.Filters
{
    public class ValidateModelAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var validationArguments = GetValidatorArguments(context).ToArray();
            if (validationArguments.Any(x => x.IsRequired && x.Value == null))
            {
                context.Result = GetBadRequestActionResult();
                return;
            }

            var validationResults = Validate(context, validationArguments.Where(x=>x.Value != null)).Where(x => !x.IsValid).ToArray();
            if (validationResults.Any())
            {
                context.Result = GetUnprocessableEntityActionResult(validationResults.SelectMany(x => x.Errors));
            }
        }

        protected virtual IEnumerable<ValidationResult> Validate(ActionExecutingContext context)
        {
            return Validate(context, context.ActionDescriptor.Parameters.Select(x => new ValidatorArgument
            {
                Name = x.Name,
                ValueType = x.ParameterType,
                Value = context.ActionArguments.FirstOrDefault(y => y.Key == x.Name).Value
            }));
        }

        protected virtual IEnumerable<ValidatorArgument> GetValidatorArguments(ActionExecutingContext context) => context.ActionDescriptor.Parameters.OfType<ControllerParameterDescriptor>().Select(x => new ValidatorArgument
        {
            Name = x.Name,
            ValueType = x.ParameterType,
            Value = context.ActionArguments.FirstOrDefault(y => y.Key == x.Name).Value,
            IsRequired = x.ParameterInfo.CustomAttributes.Any(y => y.AttributeType == typeof(RequiredAttribute))
        });

        protected virtual IEnumerable<ValidationResult> Validate(ActionExecutingContext context, IEnumerable<ValidatorArgument> arguments)
        {
            var validatorFactory = context
                .HttpContext
                .RequestServices
                .GetService<IValidatorFactory>();

            var results = arguments
                .Where(argument => argument?.ValueType != null)
                .Select(argument => validatorFactory.GetValidator(argument.ValueType)?.Validate(argument.Value))
                .Where(result => result != null)
                .ToArray();

            return results;
        }

        protected virtual IActionResult GetBadRequestActionResult()
        {
            return new ObjectResult(new ErrorResponse
            {
                Code = "bad_parameters",
                Message = "Invalid input parameters",
                Errors = new []
                {
                    new Error
                    {
                        Code = "bad_parameters",
                        Message = "Invalid type format of one or many model fields"
                    }, 
                }
            })
            {
                StatusCode = 400
            };
        }

        protected virtual IActionResult GetUnprocessableEntityActionResult(IEnumerable<ValidationFailure> errors)
        {
            return new ObjectResult(new ErrorResponse
            {
                Code = "unprocessable_entity",
                Message = "Invalid entity or business request",
                Errors = errors.Select(GetError).ToArray()
            })
            {
                StatusCode = 422
            };
        }

        protected virtual Error GetError(ValidationFailure error)
        {
            return new Error
            {
                Message = error.ErrorMessage,
                Field = error.PropertyName,
                Code = error.ErrorCode
            };
        }

        protected class ValidatorArgument
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public Type ValueType { get; set; }
            public bool IsRequired { get; set; }
        }
    }
}
