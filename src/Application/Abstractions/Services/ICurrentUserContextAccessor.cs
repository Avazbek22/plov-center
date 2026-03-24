namespace PlovCenter.Application.Abstractions.Services;

public interface ICurrentUserContextAccessor
{
    CurrentUserContext GetCurrentUser();
}
