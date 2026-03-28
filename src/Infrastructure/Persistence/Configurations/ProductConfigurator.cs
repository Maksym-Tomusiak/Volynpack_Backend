using Domain.PackageTypes;
using Domain.Products;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfigurator : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new ProductId(x))
            .IsRequired();

        builder.Property(x => x.Title)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();
        
        builder.Property(x => x.PackageTypeId)
            .HasConversion(x => x.Value, x => new PackageTypeId(x)) // Заміни на свій тип ID
            .IsRequired();
        
        builder.HasOne(x => x.Type)
            .WithMany()
            .HasForeignKey(x => x.PackageTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Мапінг JSON-колекції SuitableFor
        builder.OwnsMany(x => x.SuitableFor, a =>
        {
            a.ToJson(); // Вказуємо EF Core зберігати це як JSON
            
            a.Property(p => p.Title)
                .HasConversion(new LocalizedStringConverter())
                .IsRequired();
                
            a.Property(p => p.Description)
                .HasConversion(new LocalizedStringConverter())
                .IsRequired();
        });

        // Мапінг JSON-колекції GeneralCharacteristics
        builder.OwnsMany(x => x.GeneralCharacteristics, a =>
        {
            a.ToJson();
            
            a.Property(p => p.Title)
                .HasConversion(new LocalizedStringConverter())
                .IsRequired();
                
            a.Property(p => p.Description)
                .HasConversion(new LocalizedStringConverter())
                .IsRequired();
        });

        builder.HasMany(x => x.Categories)
            .WithMany()
            .UsingEntity(j => j.ToTable("product_category_links"));
    }
}