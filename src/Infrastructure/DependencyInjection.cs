using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlovCenter.Application.Abstractions.Auth;
using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Application.Abstractions.Services;
using PlovCenter.Infrastructure.Authentication;
using PlovCenter.Infrastructure.Configuration;
using PlovCenter.Infrastructure.Persistence;
using PlovCenter.Infrastructure.Persistence.Repositories;
using PlovCenter.Infrastructure.Services;

namespace PlovCenter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' was not found.");

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SeedAdminOptions>(configuration.GetSection(SeedAdminOptions.SectionName));
        services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));
        services.PostConfigure<FileStorageOptions>(options =>
        {
            options.RootPath = Path.IsPathRooted(options.RootPath)
                ? options.RootPath
                : Path.Combine(environment.ContentRootPath, "wwwroot", options.RootPath);
        });

        services.AddDbContext<PlovCenterDbContext>(options =>
        {
            options.UseNpgsql(connectionString, builder => builder.MigrationsAssembly(typeof(PlovCenterDbContext).Assembly.FullName));
        });

        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<PlovCenterDbContext>());
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IDishRepository, DishRepository>();
        services.AddScoped<ISiteContentRepository, SiteContentRepository>();

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<ICurrentUserContextAccessor, HttpCurrentUserContextAccessor>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        services.AddScoped<IAdminTokenService, JwtAdminTokenService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }
}
