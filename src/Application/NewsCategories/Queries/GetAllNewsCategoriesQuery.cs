using Application.Common.Interfaces.Queries;
using Domain.NewsCategories;

namespace Application.NewsCategories.Queries;

public record GetAllNewsCategoriesQuery;

public static class GetAllNewsCategoriesQueryHandler
{
    public static async Task<IReadOnlyList<NewsCategory>> Handle(
        GetAllNewsCategoriesQuery query,
        INewsCategoryQueries newsCategoryQueries,
        CancellationToken cancellationToken)
    {
        return await newsCategoryQueries.GetAll(cancellationToken);
    }
}

