using FluentValidation;
using GlobalErrorHandlingValidation.API.Responses;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace GlobalErrorHandlingValidation.API.Exceptions;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct)
    {
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var statusError = GetStatusCode(exception);

        var problemDetails = new ApiBadRequestResponse
        (
            (int)statusError,
            statusError.ToString(),
            new ErrorDetail
            (
                httpContext.Request.Path,
                GetErrors(exception)
            )
        );

        httpContext.Response.StatusCode = problemDetails.Status;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, ct);

        return true;
    }

    private static string GetErrors(Exception exception) => exception switch
    {
        Exception ex when ex is ValidationException appException => appException.Message,
        _ => $"{exception!.Message} - InternalError - {exception!.InnerException?.Message}"
    };

    private static HttpStatusCode GetStatusCode(Exception exception) => exception switch
    {
        Exception ex when ex is ValidationException => HttpStatusCode.BadRequest,
        _ => HttpStatusCode.InternalServerError
    };
}