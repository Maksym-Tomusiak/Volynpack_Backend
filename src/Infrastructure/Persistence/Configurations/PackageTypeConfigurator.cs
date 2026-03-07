using Domain.PackageTypes;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PackageTypeConfigurator : IEntityTypeConfiguration<PackageType>
{
    public void Configure(EntityTypeBuilder<PackageType> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new PackageTypeId(x))
            .IsRequired();

        builder.Property(x => x.Title)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.ImageIconUrl)
            .IsRequired();
    }
}
