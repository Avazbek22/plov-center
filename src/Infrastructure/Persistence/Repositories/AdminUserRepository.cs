using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Repositories;

internal sealed class AdminUserRepository(PlovCenterDbContext dbContext) : IAdminUserRepository
{
    public Task<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.AdminUsers.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var normalizedUsername = username.Trim().ToLowerInvariant();
        return dbContext.AdminUsers.FirstOrDefaultAsync(user => user.Username == normalizedUsername, cancellationToken);
    }

    public void Add(AdminUser adminUser)
    {
        dbContext.AdminUsers.Add(adminUser);
    }
}
