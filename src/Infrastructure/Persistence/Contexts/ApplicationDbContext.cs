using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Contexts;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Dish> Dishes => Set<Dish>();

    public DbSet<DishPhoto> DishPhotos => Set<DishPhoto>();

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    public DbSet<SiteContentEntry> SiteContentEntries => Set<SiteContentEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
