using Application.Common.Interfaces.Queries;
using Application.News.Exceptions;
using LanguageExt;

namespace Application.News.Queries;

public record GetImportantNewsQuery;

public static class GetImportantNewsQueryHandler
{
    public static async Task<Either<NewsException, Domain.News.News>> Handle(
        GetImportantNewsQuery query,
        INewsQueries newsQueries,
        CancellationToken cancellationToken)
    {
        var result = await newsQueries.GetImportant(cancellationToken);
        return result.Match<Either<NewsException, Domain.News.News>>(
            news => news,
            () => new NewsNotFoundException(Guid.Empty));
    }
}

