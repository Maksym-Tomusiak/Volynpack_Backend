using Domain.PackageMaterials;
using Domain.PackageTypes;
using Domain.Products;
using Domain.ProductVariants;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductVariantConfigurator : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new ProductVariantId(x))
            .IsRequired();

        // Зовнішні ключі
        builder.Property(x => x.ProductId)
            .HasConversion(x => x.Value, x => new ProductId(x))
            .IsRequired();

        builder.Property(x => x.PackageMaterialId)
            .HasConversion(x => x.Value, x => new PackageMaterialId(x))
            .IsRequired();
        
        builder.Property(x => x.SeoUrl)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();
        
        builder.Property(x => x.Availability)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();

        // Оновлені поля
        builder.Property(x => x.Density).IsRequired(); // Тепер це просто int
        builder.Property(x => x.LoadCapacity).HasPrecision(10, 2).IsRequired();

        // Числові значення (Розміри та ціна)
        builder.Property(x => x.Height).HasPrecision(10, 2).IsRequired();
        builder.Property(x => x.Width).HasPrecision(10, 2).IsRequired();
        builder.Property(x => x.Depth).HasPrecision(10, 2).IsRequired(false); // Nullable
        
        builder.Property(x => x.PricePerPiece).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.QuantityPerPackage).IsRequired();
        builder.Property(x => x.IsPopular)
            .IsRequired()
            .HasDefaultValue(false);

        // Зв'язки
        builder.HasOne(x => x.Product)
            .WithMany() // Якщо в класі Product є ICollection<ProductVariant>, вкажи .WithMany(x => x.Variants)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade); // При видаленні Product видаляються і всі варіації

        builder.HasOne(x => x.Material)
            .WithMany()
            .HasForeignKey(x => x.PackageMaterialId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}