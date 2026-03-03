using Domain.News;
using Domain.NewsSections;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class NewsSectionConfigurator : IEntityTypeConfiguration<NewsSection>
{
    public void Configure(EntityTypeBuilder<NewsSection> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new NewsSectionId(x))
            .IsRequired();

        builder.Property(x => x.Title)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.Content)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.Order)
            .IsRequired();

        builder.Property(x => x.NewsId)
            .HasConversion(x => x.Value, x => new NewsId(x))
            .IsRequired();

        builder.HasOne(x => x.News)
            .WithMany(x => x.Sections)
            .HasForeignKey(x => x.NewsId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

