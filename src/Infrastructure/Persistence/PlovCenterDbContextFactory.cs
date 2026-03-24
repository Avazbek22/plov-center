using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PlovCenter.Infrastructure.Persistence;

public sealed class PlovCenterDbContextFactory : IDesignTimeDbContextFactory<PlovCenterDbContext>
{
    public PlovCenterDbContext CreateDbContext(string[] args)
    {
        var webApiPath = ResolveWebApiPath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(webApiPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' was not found.");

        var optionsBuilder = new DbContextOptionsBuilder<PlovCenterDbContext>();
        optionsBuilder.UseNpgsql(connectionString, builder => builder.MigrationsAssembly(typeof(PlovCenterDbContext).Assembly.FullName));

        return new PlovCenterDbContext(optionsBuilder.Options);
    }

    private static string ResolveWebApiPath()
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (currentDirectory is not null)
        {
            var candidate = Path.Combine(currentDirectory.FullName, "src", "WebApi");

            if (File.Exists(Path.Combine(candidate, "appsettings.json")))
            {
                return candidate;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new InvalidOperationException("Could not resolve the WebApi project path for design-time DbContext creation.");
    }
}
