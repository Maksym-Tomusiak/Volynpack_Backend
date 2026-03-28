using Domain.Hashtags;

namespace Application.Common.Interfaces.Repositories;

public interface IHashtagRepository
{
    Task<Hashtag> Add(Hashtag hashtag, CancellationToken cancellationToken);
    Task<Hashtag> Update(Hashtag hashtag, CancellationToken cancellationToken);
    Task<Hashtag> Delete(Hashtag hashtag, CancellationToken cancellationToken);
}

