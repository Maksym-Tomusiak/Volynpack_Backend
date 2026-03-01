using Domain.Subscriptions;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SubscriptionConfigurator : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new SubscriptionId(x))
            .IsRequired();
        
        builder.Property(x => x.UnsubscribeToken)
            .IsRequired();
        
        builder.Property(x => x.Email)
            .HasColumnType("varchar(255)")
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter());
    }
}