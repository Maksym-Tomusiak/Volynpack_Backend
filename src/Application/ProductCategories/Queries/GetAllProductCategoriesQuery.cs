using Application.Common.Interfaces.Queries;
using Domain.ProductCategories;

namespace Application.ProductCategories.Queries;

public record GetAllProductCategoriesQuery;

public static class GetAllProductCategoriesQueryHandler
{
    public static async Task<IReadOnlyList<ProductCategory>> Handle(
        GetAllProductCategoriesQuery query,
        IProductCategoryQueries queries,
        CancellationToken cancellationToken)
    {
        return await queries.GetAll(cancellationToken);
    }
}