using Application.Common.Interfaces.Queries;
using Domain;

namespace Application.News.Queries;

public record GetAllNewsSeoUrlsQuery();

public static class GetAllNewsSeoUrlsQueryHandler
{
    public static async Task<IReadOnlyList<LocalizedString>> Handle(
        GetAllNewsSeoUrlsQuery query,
        INewsQueries queries,
        CancellationToken cancellationToken)
    {
        return await queries.GetAllSeoUrls(cancellationToken);
    }
}
