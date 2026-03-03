using Application.Common.Interfaces.Queries;
using Application.News.Exceptions;
using Domain.News;
using LanguageExt;

namespace Application.News.Queries;

public record GetSimilarNewsQuery(Guid NewsId, int Count = 3);

public static class GetSimilarNewsQueryHandler
{
    public static async Task<Either<NewsException, IReadOnlyList<Domain.News.News>>> Handle(
        GetSimilarNewsQuery query,
        INewsQueries newsQueries,
        CancellationToken cancellationToken)
    {
        var newsId = new NewsId(query.NewsId);
        var source = await newsQueries.GetById(newsId, cancellationToken);
        if (source.IsNone)
            return new NewsNotFoundException(query.NewsId);

        var similar = await newsQueries.GetSimilar(newsId, query.Count, cancellationToken);
        return Either<NewsException, IReadOnlyList<Domain.News.News>>.Right(similar);
    }
}

