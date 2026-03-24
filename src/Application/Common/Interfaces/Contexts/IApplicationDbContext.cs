using Microsoft.EntityFrameworkCore;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Common.Interfaces.Contexts;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }

    DbSet<Dish> Dishes { get; }

    DbSet<AdminUser> AdminUsers { get; }

    DbSet<SiteContentEntry> SiteContentEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
