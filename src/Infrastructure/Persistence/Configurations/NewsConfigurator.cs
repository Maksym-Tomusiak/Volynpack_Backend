using Domain.News;
using Domain.NewsCategories;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class NewsConfigurator : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new NewsId(x))
            .IsRequired();

        builder.Property(x => x.Title)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.SeoUrl)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.PhotoUrl)
            .HasColumnType("varchar(500)")
            .IsRequired();

        builder.Property(x => x.Preface)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb");

        builder.Property(x => x.Afterword)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb");

        builder.Property(x => x.CtaButtonText)
            .HasConversion(new LocalizedStringConverter())
            .HasColumnType("jsonb");

        builder.Property(x => x.CtaButtonLink)
            .HasColumnType("varchar(500)");

        builder.Property(x => x.IsImportant)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(x => x.UpdatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(x => x.CategoryId)
            .HasConversion(x => x.Value, x => new NewsCategoryId(x))
            .IsRequired();

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Sections)
            .WithOne(x => x.News)
            .HasForeignKey(x => x.NewsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Hashtags)
            .WithMany()
            .UsingEntity(j => j.ToTable("NewsHashtags"));
    }
}
