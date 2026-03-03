using Domain.NewsSections;

namespace Application.Common.Interfaces.Repositories;

public interface INewsSectionRepository
{
    Task<NewsSection> Add(NewsSection section, CancellationToken cancellationToken);
    Task<NewsSection> Update(NewsSection section, CancellationToken cancellationToken);
    Task<NewsSection> Delete(NewsSection section, CancellationToken cancellationToken);
    Task DeleteAllByNewsId(Domain.News.NewsId newsId, CancellationToken cancellationToken);
}

