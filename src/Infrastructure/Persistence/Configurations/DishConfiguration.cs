using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Configurations;

internal sealed class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.ToTable("dishes");

        builder.HasKey(static dish => dish.Id);

        builder.Property(static dish => dish.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(static dish => dish.Description)
            .HasMaxLength(2000);

        builder.Property(static dish => dish.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(static dish => dish.PhotoPath)
            .HasMaxLength(512);

        builder.Property(static dish => dish.SortOrder).IsRequired();
        builder.Property(static dish => dish.IsVisible).IsRequired();
        builder.Property(static dish => dish.CreatedUtc).IsRequired();
        builder.Property(static dish => dish.UpdatedUtc).IsRequired();

        builder.HasIndex(static dish => new { dish.CategoryId, dish.SortOrder });
    }
}
