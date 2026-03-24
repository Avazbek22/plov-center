using System.Text.Json;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.WebApi.Common;

namespace PlovCenter.WebApi.Middleware;

public sealed class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException exception)
        {
            await WriteErrorAsync(
                context,
                exception.StatusCode,
                exception.ErrorCode,
                exception.Message,
                exception is RequestValidationException validationException ? validationException.Errors : null);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing request {TraceId}", context.TraceIdentifier);

            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "server_error",
                "An unexpected server error occurred.",
                null);
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? errors)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new ApiErrorResponse(code, message, context.TraceIdentifier, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, SerializerOptions));
    }
}
