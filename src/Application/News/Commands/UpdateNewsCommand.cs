using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.News.Exceptions;
using Domain.Hashtags;
using Domain.News;
using Domain.NewsCategories;
using Domain.NewsSections;
using LanguageExt;
using Microsoft.AspNetCore.Http;

namespace Application.News.Commands;

public record UpdateNewsCommand(
    Guid Id,
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
    IReadOnlyList<UpdateNewsSectionDto> Sections);

public record UpdateNewsSectionDto(string TitleUk, string TitleEn, string ContentUk, string ContentEn, int Order);

public static class UpdateNewsCommandHandler
{
    public static async Task<Either<NewsException, Domain.News.News>> Handle(
        UpdateNewsCommand command,
        INewsRepository newsRepository,
        INewsQueries newsQueries,
        INewsSectionRepository newsSectionRepository,
        INewsCategoryQueries newsCategoryQueries,
        IHashtagQueries hashtagQueries,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        var newsId = new NewsId(command.Id);
        var existing = await newsQueries.GetById(newsId, cancellationToken);
        if (existing.IsNone)
            return new NewsNotFoundException(command.Id);

        var categoryId = new NewsCategoryId(command.CategoryId);
        var category = await newsCategoryQueries.GetById(categoryId, cancellationToken);
        if (category.IsNone)
            return new NewsCategoryNotFoundException(command.CategoryId);

        var hashtagIds = command.HashtagIds.Select(x => new HashtagId(x)).ToList();
        var hashtags = await hashtagQueries.GetByIds(hashtagIds, cancellationToken);

        try
        {
            var news = existing.First();

            string photoUrl;
            if (command.Photo is not null)
            {
                // Delete the old image before saving the new one
                if (!string.IsNullOrEmpty(news.PhotoUrl))
                    await fileService.DeleteFileAsync(news.PhotoUrl, "news", cancellationToken);

                var fileName = await fileService.SaveFileAsync(command.Photo, "news", cancellationToken);
                const string requestPath = "/uploads/news";
                photoUrl = $"{requestPath}/{fileName}";
            }
            else
            {
                photoUrl = news.PhotoUrl;
            }

            var title = new Domain.LocalizedString(command.TitleUk, command.TitleEn);
            var seoUrl = new Domain.LocalizedString(command.SeoUrlUk, command.SeoUrlEn);
            var preface = new Domain.LocalizedString(command.PrefaceUk, command.PrefaceEn);
            var afterword = new Domain.LocalizedString(command.AfterworldUk, command.AfterworldEn);
            var ctaText = new Domain.LocalizedString(command.CtaButtonTextUk, command.CtaButtonTextEn);

            news.Update(title, seoUrl, photoUrl, preface, afterword, ctaText, command.CtaButtonLink, command.IsImportant, categoryId);
            news.UpdateHashtags(hashtags);

            // Replace sections
            await newsSectionRepository.DeleteAllByNewsId(newsId, cancellationToken);
            foreach (var sectionDto in command.Sections.OrderBy(s => s.Order))
            {
                var sectionTitle = new Domain.LocalizedString(sectionDto.TitleUk, sectionDto.TitleEn);
                var sectionContent = new Domain.LocalizedString(sectionDto.ContentUk, sectionDto.ContentEn);
                var section = NewsSection.New(sectionTitle, sectionContent, sectionDto.Order, newsId);
                await newsSectionRepository.Add(section, cancellationToken);
            }

            var result = await newsRepository.Update(news, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new NewsUnknownException(command.Id, ex);
        }
    }
}

