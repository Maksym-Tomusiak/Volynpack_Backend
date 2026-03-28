using Application.Common.Interfaces.Queries;
using Application.News.Exceptions;
using LanguageExt;

namespace Application.News.Queries;

public record GetNewsBySeoUrlQuery(string SeoUrl);

public static class GetNewsBySeoUrlQueryHandler
{
    public static async Task<Either<NewsException, Domain.News.News>> Handle(
        GetNewsBySeoUrlQuery query,
        INewsQueries newsQueries,
        CancellationToken cancellationToken)
    {
        var result = await newsQueries.GetBySeoUrl(query.SeoUrl, cancellationToken);
        return result.Match<Either<NewsException, Domain.News.News>>(
            news => news,
            () => new NewsNotFoundException(Guid.Empty));
    }
}

