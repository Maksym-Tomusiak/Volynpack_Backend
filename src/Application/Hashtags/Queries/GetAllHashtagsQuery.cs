using Application.Common.Interfaces.Queries;
using Domain.Hashtags;

namespace Application.Hashtags.Queries;

public record GetAllHashtagsQuery;

public static class GetAllHashtagsQueryHandler
{
    public static async Task<IReadOnlyList<Hashtag>> Handle(
        GetAllHashtagsQuery query,
        IHashtagQueries hashtagQueries,
        CancellationToken cancellationToken)
    {
        return await hashtagQueries.GetAll(cancellationToken);
    }
}

