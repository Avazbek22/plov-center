namespace PlovCenter.Application.Common.Exceptions;

public abstract class AppException(string errorCode, string message, int statusCode) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;

    public int StatusCode { get; } = statusCode;
}
