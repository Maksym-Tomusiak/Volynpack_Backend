using Domain.Orders;
using Domain.PrintingOptions;
using Domain.ProductVariants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderItemConfigurator : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new OrderItemId(x))
            .IsRequired();

        builder.Property(x => x.OrderId)
            .HasConversion(x => x.Value, x => new OrderId(x))
            .IsRequired();

        builder.Property(x => x.ProductVariantId)
            .HasConversion(x => x.Value, x => new ProductVariantId(x))
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.PrintingOptionId)
            .HasConversion(x => x.Value, x => new PrintingOptionId(x))
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.PrintingOption)
            .WithMany()
            .HasForeignKey(x => x.PrintingOptionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ProductVariant)
            .WithMany()
            .HasForeignKey(x => x.ProductVariantId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
