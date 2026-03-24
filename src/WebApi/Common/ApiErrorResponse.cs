namespace PlovCenter.WebApi.Common;

public sealed record ApiErrorResponse(
    string Code,
    string Message,
    string TraceId,
    IReadOnlyDictionary<string, string[]>? Errors = null);
