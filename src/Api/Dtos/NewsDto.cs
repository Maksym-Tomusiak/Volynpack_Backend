using Domain.Hashtags;
using Domain.News;
using Domain.NewsCategories;
using Domain.NewsSections;
using Domain.PackageMaterials;
using Domain.PackageTypes;
using Microsoft.AspNetCore.Http;

namespace Api.Dtos;

public record LocalizedStringDto(string Uk, string En);

// ── NewsSection ──────────────────────────────────────────────────────────────

public record NewsSectionDto(
    Guid Id,
    LocalizedStringDto Title,
    LocalizedStringDto Content,
    int Order)
{
    public static NewsSectionDto FromDomainModel(NewsSection section) =>
        new(section.Id.Value,
            new LocalizedStringDto(section.Title.Uk, section.Title.En),
            new LocalizedStringDto(section.Content.Uk, section.Content.En),
            section.Order);
}

// ── Hashtag ──────────────────────────────────────────────────────────────────

public record HashtagDto(Guid Id, LocalizedStringDto Name)
{
    public static HashtagDto FromDomainModel(Hashtag hashtag) =>
        new(hashtag.Id.Value, new LocalizedStringDto(hashtag.Name.Uk, hashtag.Name.En));
}

public record HashtagCreateDto(string NameUk, string NameEn);
public record HashtagUpdateDto(string NameUk, string NameEn);

// ── NewsCategory ─────────────────────────────────────────────────────────────

public record NewsCategoryDto(Guid Id, LocalizedStringDto Title)
{
    public static NewsCategoryDto FromDomainModel(NewsCategory category) =>
        new(category.Id.Value, new LocalizedStringDto(category.Title.Uk, category.Title.En));
}

public record NewsCategoryCreateDto(string TitleUk, string TitleEn);
public record NewsCategoryUpdateDto(string TitleUk, string TitleEn);

// ── PackageType ──────────────────────────────────────────────────────────────

public record PackageTypeDto(Guid Id, LocalizedStringDto Title, string ImageIconUrl, string ImageOverlayUrl)
{
    public static PackageTypeDto FromDomainModel(PackageType packageType) =>
        new(packageType.Id.Value,
            new LocalizedStringDto(packageType.Title.Uk, packageType.Title.En),
            packageType.ImageIconUrl,
            packageType.ImageOverlayUrl);
}

public record PackageTypeCreateDto(string TitleUk, string TitleEn, IFormFile ImageIcon, IFormFile ImageOverlay);
public record PackageTypeUpdateDto(string TitleUk, string TitleEn, IFormFile? ImageIcon, IFormFile? ImageOverlay);

// ── PackageMaterial ──────────────────────────────────────────────────────────

public record PackageMaterialDto(Guid Id, LocalizedStringDto Title)
{
    public static PackageMaterialDto FromDomainModel(PackageMaterial material) =>
        new(material.Id.Value,
            new LocalizedStringDto(material.Title.Uk, material.Title.En));
}

public record PackageMaterialCreateDto(string TitleUk, string TitleEn);
public record PackageMaterialUpdateDto(string TitleUk, string TitleEn);

// ── News ─────────────────────────────────────────────────────────────────────

public record NewsDto(
    Guid Id,
    LocalizedStringDto Title,
    LocalizedStringDto SeoUrl,
    string PhotoUrl,
    LocalizedStringDto Preface,
    LocalizedStringDto Afterword,
    LocalizedStringDto CtaButtonText,
    string CtaButtonLink,
    bool IsImportant,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    NewsCategoryDto? Category,
    IReadOnlyList<NewsSectionDto> Sections,
    IReadOnlyList<HashtagDto> Hashtags)
{
    public static NewsDto FromDomainModel(News news) =>
        new(news.Id.Value,
            new LocalizedStringDto(news.Title.Uk, news.Title.En),
            new LocalizedStringDto(news.SeoUrl.Uk, news.SeoUrl.En),
            news.PhotoUrl,
            new LocalizedStringDto(news.Preface.Uk, news.Preface.En),
            new LocalizedStringDto(news.Afterword.Uk, news.Afterword.En),
            new LocalizedStringDto(news.CtaButtonText.Uk, news.CtaButtonText.En),
            news.CtaButtonLink,
            news.IsImportant,
            news.CreatedAt,
            news.UpdatedAt,
            news.Category is null ? null : NewsCategoryDto.FromDomainModel(news.Category),
            news.Sections.Select(NewsSectionDto.FromDomainModel).ToList(),
            news.Hashtags.Select(HashtagDto.FromDomainModel).ToList());
}

public record NewsSectionCreateDto(string TitleUk, string TitleEn, string ContentUk, string ContentEn, int Order);

public record NewsCreateDto(
    string TitleUk,
    string TitleEn,
    string SeoUrlUk,
    string SeoUrlEn,
    IFormFile Photo,
    string PrefaceUk,
    string PrefaceEn,
    string AfterworldUk,
    string AfterworldEn,
    string CtaButtonTextUk,
    string CtaButtonTextEn,
    string CtaButtonLink,
    bool IsImportant,
    Guid CategoryId,
    IReadOnlyList<Guid> HashtagIds,
    IReadOnlyList<NewsSectionCreateDto> Sections);

public record NewsUpdateDto(
    string TitleUk,
    string TitleEn,
    string SeoUrlUk,
    string SeoUrlEn,
    IFormFile? Photo,
    string PrefaceUk,
    string PrefaceEn,
    string AfterworldUk,
    string AfterworldEn,
    string CtaButtonTextUk,
    string CtaButtonTextEn,
    string CtaButtonLink,
    bool IsImportant,
    Guid CategoryId,
    IReadOnlyList<Guid> HashtagIds,
    IReadOnlyList<NewsSectionCreateDto> Sections);

// Lightweight list DTO (for paginated / similar – no sections)
public record NewsListItemDto(
    Guid Id,
    LocalizedStringDto Title,
    LocalizedStringDto SeoUrl,
    string PhotoUrl,
    LocalizedStringDto Preface,
    bool IsImportant,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    NewsCategoryDto? Category,
    IReadOnlyList<HashtagDto> Hashtags)
{
    public static NewsListItemDto FromDomainModel(News news) =>
        new(news.Id.Value,
            new LocalizedStringDto(news.Title.Uk, news.Title.En),
            new LocalizedStringDto(news.SeoUrl.Uk, news.SeoUrl.En),
            news.PhotoUrl,
            new LocalizedStringDto(news.Preface.Uk, news.Preface.En),
            news.IsImportant,
            news.CreatedAt,
            news.UpdatedAt,
            news.Category is null ? null : NewsCategoryDto.FromDomainModel(news.Category),
            news.Hashtags.Select(HashtagDto.FromDomainModel).ToList());
}

