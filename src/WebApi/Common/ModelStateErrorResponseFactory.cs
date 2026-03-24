using Microsoft.AspNetCore.Mvc;

namespace PlovCenter.WebApi.Common;

internal static class ModelStateErrorResponseFactory
{
    public static IActionResult Create(ActionContext context)
    {
        var errors = context.ModelState
            .Where(static entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                static entry => string.IsNullOrWhiteSpace(entry.Key) ? "$" : entry.Key,
                static entry => entry.Value!.Errors
                    .Select(static error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "The input was not valid." : error.ErrorMessage)
                    .Distinct()
                    .ToArray());

        var payload = ApiErrorResponseWriter.CreatePayload(
            context.HttpContext,
            "validation_error",
            "One or more validation errors occurred.",
            errors);

        return new BadRequestObjectResult(payload);
    }
}
