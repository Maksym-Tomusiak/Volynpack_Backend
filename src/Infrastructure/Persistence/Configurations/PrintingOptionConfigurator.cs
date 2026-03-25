using Domain.PrintingOptions;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PrintingOptionConfigurator : IEntityTypeConfiguration<PrintingOption>
{
    public void Configure(EntityTypeBuilder<PrintingOption> builder)
    {
        builder.ToTable("PrintingOptions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new PrintingOptionId(x))
            .IsRequired();

        builder.Property(x => x.Name)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();
    }
}