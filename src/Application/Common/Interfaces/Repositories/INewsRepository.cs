using Domain.News;

namespace Application.Common.Interfaces.Repositories;

public interface INewsRepository
{
    Task<Domain.News.News> Add(Domain.News.News news, CancellationToken cancellationToken);
    Task<Domain.News.News> Update(Domain.News.News news, CancellationToken cancellationToken);
    Task<Domain.News.News> Delete(Domain.News.News news, CancellationToken cancellationToken);
}

