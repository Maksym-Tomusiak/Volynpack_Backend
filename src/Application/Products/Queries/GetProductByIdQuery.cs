using Application.Common.Interfaces.Queries;
using Application.Products.Exceptions;
using Domain.Products;
using LanguageExt;

namespace Application.Products.Queries;

public record GetProductByIdQuery(Guid Id);

public static class GetProductByIdQueryHandler
{
    public static async Task<Either<ProductException, Product>> Handle(
        GetProductByIdQuery query,
        IProductQueries productQueries,
        CancellationToken cancellationToken)
    {
        var productId = new ProductId(query.Id);
        var resultOption = await productQueries.GetById(productId, cancellationToken);
        
        return resultOption.Match<Either<ProductException, Product>>(
            product => product,
            () => new ProductNotFoundException(query.Id));
    }
}