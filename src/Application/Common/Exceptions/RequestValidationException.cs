namespace PlovCenter.Application.Common.Exceptions;

public sealed class RequestValidationException(IDictionary<string, string[]> errors)
    : AppException("validation_error", "One or more validation errors occurred.", 400)
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>(errors);
}
