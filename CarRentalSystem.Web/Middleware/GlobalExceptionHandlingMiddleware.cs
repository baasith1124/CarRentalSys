using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using FluentValidation;

namespace CarRentalSystem.Web.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new object();
            var statusCode = HttpStatusCode.InternalServerError;

            // Handle FluentValidation exceptions
            if (exception is ValidationException validationException)
            {
                statusCode = HttpStatusCode.BadRequest;
                response = new
                {
                    StatusCode = (int)statusCode,
                    Message = "Validation failed.",
                    Errors = validationException.Errors.Select(e => new
                    {
                        PropertyName = e.PropertyName,
                        ErrorMessage = e.ErrorMessage,
                        AttemptedValue = e.AttemptedValue
                    })
                };
            }
            else
            {
                // Handle other exceptions
                response = new
                {
                    StatusCode = (int)statusCode,
                    Message = "An internal server error occurred.",
                    Details = exception.Message // Remove in production
                };
            }

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            // For AJAX requests, return JSON
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
            else
            {
                // For regular requests, redirect to error page
                context.Response.Redirect("/Home/Error");
            }
        }
    }

    public static class GlobalExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}