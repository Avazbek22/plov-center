using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Configurations;

internal sealed class DishPhotoConfiguration : IEntityTypeConfiguration<DishPhoto>
{
    public void Configure(EntityTypeBuilder<DishPhoto> builder)
    {
        builder.ToTable("dish_photos");

        builder.HasKey(static photo => photo.Id);

        builder.Property(static photo => photo.RelativePath)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(static photo => photo.SortOrder).IsRequired();
        builder.Property(static photo => photo.CreatedUtc).IsRequired();
        builder.Property(static photo => photo.UpdatedUtc).IsRequired();

        builder.HasOne(static photo => photo.Dish)
            .WithMany(static dish => dish.Photos)
            .HasForeignKey(static photo => photo.DishId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(static photo => new { photo.DishId, photo.SortOrder });
    }
}
