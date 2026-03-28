using Domain.PackageFittings;
using Domain.PackageMaterials;
using Domain.PackageTypes;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PackageFittingConfigurator : IEntityTypeConfiguration<PackageFitting>
{
    public void Configure(EntityTypeBuilder<PackageFitting> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new PackageFittingId(x))
            .IsRequired();

        builder.Property(x => x.TypeId)
            .HasConversion(x => x.Value, x => new PackageTypeId(x))
            .IsRequired();

        builder.HasOne(x => x.Type)
            .WithMany()
            .HasForeignKey(x => x.TypeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.MaterialId)
            .HasConversion(x => x.Value, x => new PackageMaterialId(x))
            .IsRequired();

        builder.HasOne(x => x.Material)
            .WithMany()
            .HasForeignKey(x => x.MaterialId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.FittingImageUrl)
            .IsRequired();
    }
}
