using Domain.DelivaryMethods;
using Domain.Orders;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DeliveryMethodConfigurator : IEntityTypeConfiguration<DeliveryMethod>
{
    public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
    {
        builder.ToTable("DeliveryMethods");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new DeliveryMethodId(x))
            .IsRequired();

        builder.Property(x => x.Name)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();
    }
}
