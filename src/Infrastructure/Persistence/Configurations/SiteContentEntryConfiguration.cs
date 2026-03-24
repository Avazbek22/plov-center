using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Configurations;

internal sealed class SiteContentEntryConfiguration : IEntityTypeConfiguration<SiteContentEntry>
{
    public void Configure(EntityTypeBuilder<SiteContentEntry> builder)
    {
        builder.ToTable("site_content_entries");

        builder.HasKey(static entry => entry.Id);

        builder.Property(static entry => entry.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(static entry => entry.Value)
            .HasMaxLength(5000);

        builder.Property(static entry => entry.CreatedUtc).IsRequired();
        builder.Property(static entry => entry.UpdatedUtc).IsRequired();

        builder.HasIndex(static entry => entry.Key).IsUnique();
    }
}
