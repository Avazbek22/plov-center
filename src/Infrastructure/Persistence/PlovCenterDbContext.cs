using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence;

public sealed class PlovCenterDbContext(DbContextOptions<PlovCenterDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Dish> Dishes => Set<Dish>();

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    public DbSet<SiteContentEntry> SiteContentEntries => Set<SiteContentEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlovCenterDbContext).Assembly);
    }
}
