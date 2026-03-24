using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Application.Abstractions.Services;
using PlovCenter.Application.Abstractions.Auth;
using PlovCenter.Application.Features.Content;
using PlovCenter.Domain.Entities;
using PlovCenter.Infrastructure.Configuration;

namespace PlovCenter.Infrastructure.Persistence;

public static class DatabaseInitialization
{
    public static async Task ApplyMigrationsAndSeedAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var scopedProvider = scope.ServiceProvider;

        var dbContext = scopedProvider.GetRequiredService<PlovCenterDbContext>();
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

        var dbContext = serviceProvider.GetRequiredService<PlovCenterDbContext>();
        var adminRepository = serviceProvider.GetRequiredService<IAdminUserRepository>();
        var passwordHashService = serviceProvider.GetRequiredService<IPasswordHashService>();
        var dateTimeProvider = serviceProvider.GetRequiredService<IDateTimeProvider>();

        var adminUser = await adminRepository.GetByUsernameAsync(options.Username, cancellationToken);

        if (adminUser is null)
        {
            adminUser = new AdminUser(options.Username, string.Empty, options.IsActive, dateTimeProvider.UtcNow);
            adminUser.UpdatePasswordHash(passwordHashService.HashPassword(adminUser, options.Password), dateTimeProvider.UtcNow);
            adminRepository.Add(adminUser);
        }
        else
        {
            adminUser.Rename(options.Username, dateTimeProvider.UtcNow);
            adminUser.SetActive(options.IsActive, dateTimeProvider.UtcNow);
            adminUser.UpdatePasswordHash(passwordHashService.HashPassword(adminUser, options.Password), dateTimeProvider.UtcNow);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedSiteContentAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var dbContext = serviceProvider.GetRequiredService<PlovCenterDbContext>();
        var siteContentRepository = serviceProvider.GetRequiredService<ISiteContentRepository>();
        var dateTimeProvider = serviceProvider.GetRequiredService<IDateTimeProvider>();

        var existingEntries = await siteContentRepository.GetByKeysAsync(SiteContentKeys.All, cancellationToken);
        var missingEntries = SiteContentKeys.All
            .Where(key => !existingEntries.ContainsKey(key))
            .Select(key => new SiteContentEntry(key, null, dateTimeProvider.UtcNow))
            .ToArray();

        if (missingEntries.Length == 0)
        {
            return;
        }

        siteContentRepository.AddRange(missingEntries);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
