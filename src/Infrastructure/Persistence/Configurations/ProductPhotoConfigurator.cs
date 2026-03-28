using Domain.ProductPhotos;
using Domain.ProductVariants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductPhotoConfigurator : IEntityTypeConfiguration<ProductPhoto>
{
    public void Configure(EntityTypeBuilder<ProductPhoto> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new ProductPhotoId(x))
            .IsRequired();

        builder.Property(x => x.ProductVariantId)
            .HasConversion(x => x.Value, x => new ProductVariantId(x))
            .IsRequired();

        builder.Property(x => x.PhotoUrl)
            .HasColumnType("varchar(500)")
            .IsRequired();

        builder.Property(x => x.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        // Зв'язок
        builder.HasOne<ProductVariant>()
            .WithMany(x => x.Photos) // Або .WithMany(x => x.Photos), якщо ти додаси колекцію фото в ProductVariant
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade); // При видаленні варіації фото також видаляються
    }
}