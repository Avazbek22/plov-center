using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PlovCenter.Application.Common.Constants;
using PlovCenter.Application.Common.Extensions;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Domain.Entities;
using PlovCenter.Infrastructure.Configuration;
using PlovCenter.Infrastructure.Persistence.Contexts;

namespace PlovCenter.Infrastructure.Persistence;

public static class DatabaseInitialization
{
    public static async Task ApplyMigrationsAndSeedAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var scopedProvider = scope.ServiceProvider;

        var dbContext = scopedProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);

        await SeedAdminAsync(scopedProvider, cancellationToken);
        await SeedSiteContentAsync(scopedProvider, cancellationToken);
    }

    private static async Task SeedAdminAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var options = serviceProvider.GetRequiredService<IOptions<SeedAdminOptions>>().Value;

        if (string.IsNullOrWhiteSpace(options.Username) || string.IsNullOrWhiteSpace(options.Password))
        {
            throw new InvalidOperationException("SeedAdmin configuration must provide both username and password.");
        }

        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHashService = serviceProvider.GetRequiredService<IPasswordHashService>();
        var dateTimeService = serviceProvider.GetRequiredService<IDateTimeService>();
        var normalizedUsername = options.Username.NormalizeTrimmedLowerInvariant();

        var adminUser = await dbContext.AdminUsers
            .FirstOrDefaultAsync(user => user.Username == normalizedUsername, cancellationToken);

        if (adminUser is null)
        {
            adminUser = new AdminUser
            {
                Id = Guid.NewGuid(),
                Username = normalizedUsername,
                IsActive = options.IsActive,
                CreatedUtc = dateTimeService.UtcNow,
                UpdatedUtc = dateTimeService.UtcNow
            };

            adminUser.PasswordHash = passwordHashService.HashPassword(adminUser, options.Password);
            dbContext.AdminUsers.Add(adminUser);
        }
        else
        {
            adminUser.Username = normalizedUsername;
            adminUser.IsActive = options.IsActive;
            adminUser.PasswordHash = passwordHashService.HashPassword(adminUser, options.Password);
            adminUser.UpdatedUtc = dateTimeService.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedSiteContentAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var dateTimeService = serviceProvider.GetRequiredService<IDateTimeService>();

        var existingEntries = await dbContext.SiteContentEntries
            .Where(entry => SiteContentKeys.All.Contains(entry.Key))
            .ToDictionaryAsync(entry => entry.Key, cancellationToken);

        var missingEntries = SiteContentKeys.All
            .Where(key => !existingEntries.ContainsKey(key))
            .Select(key => new SiteContentEntry
            {
                Id = Guid.NewGuid(),
                Key = key,
                Value = null,
                CreatedUtc = dateTimeService.UtcNow,
                UpdatedUtc = dateTimeService.UtcNow
            })
            .ToArray();

        if (missingEntries.Length == 0)
        {
            return;
        }

        dbContext.SiteContentEntries.AddRange(missingEntries);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
