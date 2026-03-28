using Domain.NewsCategories;

namespace Application.Common.Interfaces.Repositories;

public interface INewsCategoryRepository
{
    Task<NewsCategory> Add(NewsCategory category, CancellationToken cancellationToken);
    Task<NewsCategory> Update(NewsCategory category, CancellationToken cancellationToken);
    Task<NewsCategory> Delete(NewsCategory category, CancellationToken cancellationToken);
}

