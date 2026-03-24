namespace PlovCenter.Application.Common.Exceptions;

public sealed class ConflictException(string message) : AppException("conflict", message, 409);
