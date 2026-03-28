using Domain.News;
using Domain.NewsSections;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface INewsSectionQueries
{
    Task<IReadOnlyList<NewsSection>> GetByNewsId(NewsId newsId, CancellationToken cancellationToken);
    Task<Option<NewsSection>> GetById(NewsSectionId id, CancellationToken cancellationToken);
}

