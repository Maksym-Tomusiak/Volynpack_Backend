using Application.Common.Interfaces.Queries;
using Application.ProductCategories.Exceptions;
using Domain.ProductCategories;
using LanguageExt;

namespace Application.ProductCategories.Queries;

public record GetProductCategoryByIdQuery(Guid Id);

public static class GetProductCategoryByIdQueryHandler
{
    public static async Task<Either<ProductCategoryException, ProductCategory>> Handle(
        GetProductCategoryByIdQuery query,
        IProductCategoryQueries queries,
        CancellationToken cancellationToken)
    {
        var categoryId = new ProductCategoryId(query.Id);
        var resultOption = await queries.GetById(categoryId, cancellationToken);
        
        return resultOption.Match<Either<ProductCategoryException, ProductCategory>>(
            category => category,
            () => new ProductCategoryNotFoundException(query.Id));
    }
}