using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using WHMS.Api.Common.Models;
using WHMS.Domain.Exceptions;

namespace WHMS.Api.Common.ExceptionHandling;

public class ApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, response) = exception switch
        {
            NotFoundException notFound => (StatusCodes.Status404NotFound, ApiResponse.Failure(notFound.Message)),
            BusinessRuleViolationException ruleViolation => (StatusCodes.Status400BadRequest, ApiResponse.Failure(ruleViolation.Message)),
            ValidationException validation => (StatusCodes.Status400BadRequest, ApiResponse.Failure(validation.Errors.Select(e => e.ErrorMessage).ToList())),
            _ => (StatusCodes.Status500InternalServerError, ApiResponse.Failure("An unexpected error occurred."))
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
