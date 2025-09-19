using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarRentalSystem.Web.Filters
{
    public class ValidationExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException validationException)
            {
                var errors = validationException.Errors.Select(e => new
                {
                    PropertyName = e.PropertyName,
                    ErrorMessage = e.ErrorMessage,
                    AttemptedValue = e.AttemptedValue
                }).ToList();

                var result = new BadRequestObjectResult(new
                {
                    Message = "Validation failed.",
                    Errors = errors
                });

                context.Result = result;
                context.ExceptionHandled = true;
            }
        }
    }
}
