using Domain.NewsCategories;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface INewsCategoryQueries
{
    Task<IReadOnlyList<NewsCategory>> GetAll(CancellationToken cancellationToken);
    Task<Option<NewsCategory>> GetById(NewsCategoryId id, CancellationToken cancellationToken);
}

