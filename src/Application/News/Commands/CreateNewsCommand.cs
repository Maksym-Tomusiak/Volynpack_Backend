using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.News.Exceptions;
using Domain.Hashtags;
using Domain.NewsCategories;
using Domain.NewsSections;
using LanguageExt;
using Microsoft.AspNetCore.Http;

namespace Application.News.Commands;

public record CreateNewsCommand(
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
    IReadOnlyList<CreateNewsSectionDto> Sections);

public record CreateNewsSectionDto(string TitleUk, string TitleEn, string ContentUk, string ContentEn, int Order);

public static class CreateNewsCommandHandler
{
    public static async Task<Either<NewsException, Domain.News.News>> Handle(
        CreateNewsCommand command,
        INewsRepository newsRepository,
        INewsSectionRepository newsSectionRepository,
        INewsCategoryQueries newsCategoryQueries,
        IHashtagQueries hashtagQueries,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        var categoryId = new NewsCategoryId(command.CategoryId);
        var category = await newsCategoryQueries.GetById(categoryId, cancellationToken);
        if (category.IsNone)
            return new NewsCategoryNotFoundException(command.CategoryId);

        var hashtagIds = command.HashtagIds.Select(x => new HashtagId(x)).ToList();
        var hashtags = await hashtagQueries.GetByIds(hashtagIds, cancellationToken);

        try
        {
            var fileName = await fileService.SaveFileAsync(command.Photo, "news", cancellationToken);
            const string requestPath = "/uploads/news";
            var photoUrl = $"{requestPath}/{fileName}";

            var title = new Domain.LocalizedString(command.TitleUk, command.TitleEn);
            var seoUrl = new Domain.LocalizedString(command.SeoUrlUk, command.SeoUrlEn);
            var preface = new Domain.LocalizedString(command.PrefaceUk, command.PrefaceEn);
            var afterword = new Domain.LocalizedString(command.AfterworldUk, command.AfterworldEn);
            var ctaText = new Domain.LocalizedString(command.CtaButtonTextUk, command.CtaButtonTextEn);

            var news = Domain.News.News.New(title, seoUrl, photoUrl, preface, afterword, ctaText, command.CtaButtonLink, command.IsImportant, categoryId);

            news.UpdateHashtags(hashtags);

            var result = await newsRepository.Add(news, cancellationToken);

            int order = 0;
            foreach (var sectionDto in command.Sections.OrderBy(s => s.Order))
            {
                var sectionTitle = new Domain.LocalizedString(sectionDto.TitleUk, sectionDto.TitleEn);
                var sectionContent = new Domain.LocalizedString(sectionDto.ContentUk, sectionDto.ContentEn);
                var section = NewsSection.New(sectionTitle, sectionContent, sectionDto.Order == 0 ? order++ : sectionDto.Order, result.Id);
                await newsSectionRepository.Add(section, cancellationToken);
            }

            return result;
        }
        catch (Exception ex)
        {
            return new NewsUnknownException(Guid.Empty, ex);
        }
    }
}

