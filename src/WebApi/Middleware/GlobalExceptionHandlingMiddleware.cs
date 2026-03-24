using System.Text.Json;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.WebApi.Common;

namespace PlovCenter.WebApi.Middleware;

public sealed class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException exception)
        {
            await ApiErrorResponseWriter.WriteAsync(
                context,
                exception.StatusCode,
                exception.ErrorCode,
                exception.Message,
                exception is RequestValidationException validationException ? validationException.Errors : null);
        }
        catch (BadHttpRequestException)
        {
            await ApiErrorResponseWriter.WriteAsync(
                context,
                StatusCodes.Status400BadRequest,
                "validation_error",
                "The request payload is invalid.");
        }
        catch (InvalidDataException)
        {
            await ApiErrorResponseWriter.WriteAsync(
                context,
                StatusCodes.Status400BadRequest,
                "validation_error",
                "The request payload is invalid.");
        }
        catch (JsonException)
        {
            await ApiErrorResponseWriter.WriteAsync(
                context,
                StatusCodes.Status400BadRequest,
                "validation_error",
                "The request payload is invalid.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing request {TraceId}", context.TraceIdentifier);

            await ApiErrorResponseWriter.WriteAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "server_error",
                "An unexpected server error occurred.",
                null);
        }
    }
}
