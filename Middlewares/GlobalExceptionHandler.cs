using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace Acceloka.Middlewares
{
    public class GlobalExceptionHandler : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path
            };

            if (exception is ValidationException validationEx)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "One or more validation errors occurred.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

                var errors = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                problemDetails.Extensions.Add("errors", errors);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "An error occurred while processing your request.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                problemDetails.Detail = exception.Message;
            }

            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
}
