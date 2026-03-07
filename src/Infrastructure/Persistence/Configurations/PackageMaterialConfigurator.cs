using Domain.PackageMaterials;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PackageMaterialConfigurator : IEntityTypeConfiguration<PackageMaterial>
{
    public void Configure(EntityTypeBuilder<PackageMaterial> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new PackageMaterialId(x))
            .IsRequired();

        builder.Property(x => x.Title)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();
    }
}
