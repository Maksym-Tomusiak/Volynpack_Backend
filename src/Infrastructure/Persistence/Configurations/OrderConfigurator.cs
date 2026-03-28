using Domain.DelivaryMethods;
using Domain.Orders;
using Domain.OrderStatuses;
using Domain.PrintingOptions;
using Domain.ProductVariants;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderConfigurator : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new OrderId(x))
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        // Зв'язок зі статусом
        builder.Property(x => x.OrderStatusId)
            .HasConversion(x => x.Value, x => new OrderStatusId(x))
            .IsRequired();

        builder.HasOne(x => x.OrderStatus)
            .WithMany()
            .HasForeignKey(x => x.OrderStatusId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Зв'язок з методом доставки
        builder.Property(x => x.DeliveryMethodId)
            .HasConversion(x => x.Value, x => new DeliveryMethodId(x))
            .IsRequired();

        builder.HasOne(x => x.DeliveryMethod)
            .WithMany()
            .HasForeignKey(x => x.DeliveryMethodId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Town)
            .HasMaxLength(100);

        builder.Property(x => x.Branch)
            .HasMaxLength(100);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}