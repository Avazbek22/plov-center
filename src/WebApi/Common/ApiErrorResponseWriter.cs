using System.Text.Json;

namespace PlovCenter.WebApi.Common;

internal static class ApiErrorResponseWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static async Task WriteAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = CreatePayload(context, code, message, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, SerializerOptions));
    }

    public static ApiErrorResponse CreatePayload(
        HttpContext context,
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        return new ApiErrorResponse(code, message, context.TraceIdentifier, errors);
    }
}
