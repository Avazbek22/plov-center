using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Configurations;

internal sealed class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.ToTable("admin_users");

        builder.HasKey(static user => user.Id);

        builder.Property(static user => user.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(static user => user.PasswordHash).IsRequired();
        builder.Property(static user => user.IsActive).IsRequired();
        builder.Property(static user => user.CreatedUtc).IsRequired();
        builder.Property(static user => user.UpdatedUtc).IsRequired();

        builder.HasIndex(static user => user.Username).IsUnique();
    }
}
