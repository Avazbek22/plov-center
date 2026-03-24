using PlovCenter.Application.Common.Models;

namespace PlovCenter.Application.Common.Interfaces.Services;

public interface ICurrentUserService
{
    CurrentUser GetCurrentUser();
}
