using Domain.Hashtags;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IHashtagQueries
{
    Task<IReadOnlyList<Hashtag>> GetAll(CancellationToken cancellationToken);
    Task<Option<Hashtag>> GetById(HashtagId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Hashtag>> GetByIds(IEnumerable<HashtagId> ids, CancellationToken cancellationToken);
}

