using Application.Common.Interfaces.Queries;
using Application.News.Exceptions;
using Domain.News;
using LanguageExt;

namespace Application.News.Queries;

public record GetNewsByIdQuery(Guid Id);

public static class GetNewsByIdQueryHandler
{
    public static async Task<Either<NewsException, Domain.News.News>> Handle(
        GetNewsByIdQuery query,
        INewsQueries newsQueries,
        CancellationToken cancellationToken)
    {
        var newsId = new NewsId(query.Id);
        var result = await newsQueries.GetById(newsId, cancellationToken);
        return result.Match<Either<NewsException, Domain.News.News>>(
            news => news,
            () => new NewsNotFoundException(query.Id));
    }
}

