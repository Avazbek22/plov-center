using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(static category => category.Id);

        builder.Property(static category => category.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(static category => category.SortOrder).IsRequired();
        builder.Property(static category => category.IsVisible).IsRequired();
        builder.Property(static category => category.CreatedUtc).IsRequired();
        builder.Property(static category => category.UpdatedUtc).IsRequired();

        builder.HasMany(static category => category.Dishes)
            .WithOne(static dish => dish.Category)
            .HasForeignKey(static dish => dish.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(static category => category.SortOrder);
    }
}
