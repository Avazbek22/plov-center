namespace PlovCenter.Application.Common.Exceptions;

public sealed class UnauthorizedException(string message) : AppException("unauthorized", message, 401);
