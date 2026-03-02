using FluentValidation;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public sealed class ValidationMappingMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMappingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        // ValidationException is thrown by FluentValidation
        catch (ValidationException ex)
        { 
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            
            var validationFailureResponse = new ValidationFailureResponse
            {
                Errors = ex.Errors.Select(x =>
                    new ValidationResponse
                    {
                        PropertyName = x.PropertyName,
                        Message = x.ErrorMessage
                    })
            };
            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
    }
}