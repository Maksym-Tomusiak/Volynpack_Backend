using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Models;
using Application.News.Commands;
using Application.News.Exceptions;
using Application.News.Queries;
using Domain.News;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Controllers;

[ApiController]
public class NewsController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("api/news")]
    public async Task<IResult> GetNewsPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetNewsPaginatedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending);
        var result = await messageBus.InvokeAsync<PaginatedResult<News>>(query, cancellationToken);
        return Results.Ok(PaginatedResult<News>.MapFrom(result, NewsListItemDto.FromDomainModel));
    }

    [HttpGet("api/news/important")]
    public async Task<IResult> GetImportantNews(CancellationToken cancellationToken)
    {
        var query = new GetImportantNewsQuery();
        var result = await messageBus.InvokeAsync<Either<NewsException, News>>(query, cancellationToken);
        return result.Match<IResult>(
            news => Results.Ok(NewsDto.FromDomainModel(news)),
            ex => ex.ToIResult());
    }

    [HttpGet("api/news/{id:guid}")]
    public async Task<IResult> GetNewsById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetNewsByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<NewsException, News>>(query, cancellationToken);
        return result.Match<IResult>(
            news => Results.Ok(NewsDto.FromDomainModel(news)),
            ex => ex.ToIResult());
    }

    [HttpGet("api/news/seo/{seoUrl}")]
    public async Task<IResult> GetNewsBySeoUrl(string seoUrl, CancellationToken cancellationToken)
    {
        var query = new GetNewsBySeoUrlQuery(seoUrl);
        var result = await messageBus.InvokeAsync<Either<NewsException, News>>(query, cancellationToken);
        return result.Match<IResult>(
            news => Results.Ok(NewsDto.FromDomainModel(news)),
            ex => ex.ToIResult());
    }

    [HttpGet("api/news/{id:guid}/similar")]
    public async Task<IResult> GetSimilarNews(Guid id, [FromQuery] int count = 3, CancellationToken cancellationToken = default)
    {
        var query = new GetSimilarNewsQuery(id, count);
        var result = await messageBus.InvokeAsync<Either<NewsException, IReadOnlyList<News>>>(query, cancellationToken);
        return result.Match<IResult>(
            newsList => Results.Ok(newsList.Select(NewsListItemDto.FromDomainModel)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/news")]
    public async Task<IResult> CreateNews([FromForm] NewsCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreateNewsCommand(
            request.TitleUk, request.TitleEn,
            request.SeoUrlUk, request.SeoUrlEn,
            request.Photo,
            request.PrefaceUk, request.PrefaceEn,
            request.AfterworldUk, request.AfterworldEn,
            request.CtaButtonTextUk, request.CtaButtonTextEn,
            request.CtaButtonLink,
            request.IsImportant,
            request.CategoryId,
            request.HashtagIds,
            request.Sections.Select(s => new CreateNewsSectionDto(s.TitleUk, s.TitleEn, s.ContentUk, s.ContentEn, s.Order)).ToList());

        var result = await messageBus.InvokeAsync<Either<NewsException, News>>(cmd, cancellationToken);
        return result.Match<IResult>(
            news => Results.Created($"/api/news/{news.Id.Value}", NewsDto.FromDomainModel(news)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/news/{id:guid}")]
    public async Task<IResult> UpdateNews(Guid id, [FromForm] NewsUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateNewsCommand(
            id,
            request.TitleUk, request.TitleEn,
            request.SeoUrlUk, request.SeoUrlEn,
            request.Photo,
            request.PrefaceUk, request.PrefaceEn,
            request.AfterworldUk, request.AfterworldEn,
            request.CtaButtonTextUk, request.CtaButtonTextEn,
            request.CtaButtonLink,
            request.IsImportant,
            request.CategoryId,
            request.HashtagIds,
            request.Sections.Select(s => new UpdateNewsSectionDto(s.TitleUk, s.TitleEn, s.ContentUk, s.ContentEn, s.Order)).ToList());

        var result = await messageBus.InvokeAsync<Either<NewsException, News>>(cmd, cancellationToken);
        return result.Match<IResult>(
            news => Results.Ok(NewsDto.FromDomainModel(news)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/news/{id:guid}")]
    public async Task<IResult> DeleteNews(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeleteNewsCommand(id);
        var result = await messageBus.InvokeAsync<Either<NewsException, News>>(cmd, cancellationToken);
        return result.Match<IResult>(
            news => Results.Ok(NewsDto.FromDomainModel(news)),
            ex => ex.ToIResult());
    }
}

