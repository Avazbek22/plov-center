namespace PlovCenter.Application.Common.Exceptions;

public sealed class NotFoundException(string message) : AppException("not_found", message, 404);
