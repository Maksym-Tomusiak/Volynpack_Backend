using Application.Common.Interfaces.Queries;
using Application.Hashtags.Exceptions;
using Domain.Hashtags;
using LanguageExt;

namespace Application.Hashtags.Queries;

public record GetHashtagByIdQuery(Guid Id);

public static class GetHashtagByIdQueryHandler
{
    public static async Task<Either<HashtagException, Hashtag>> Handle(
        GetHashtagByIdQuery query,
        IHashtagQueries hashtagQueries,
        CancellationToken cancellationToken)
    {
        var hashtagId = new HashtagId(query.Id);
        var result = await hashtagQueries.GetById(hashtagId, cancellationToken);
        return result.Match<Either<HashtagException, Hashtag>>(
            hashtag => hashtag,
            () => new HashtagNotFoundException(query.Id));
    }
}

