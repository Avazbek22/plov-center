namespace PlovCenter.Application.Common.Exceptions;

public sealed class ForbiddenException(string message) : AppException("forbidden", message, 403);
