using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Abstractions.Persistence;

public interface IAdminUserRepository
{
    Task<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    void Add(AdminUser adminUser);
}
