using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Products.Exceptions;
using Domain.Products;
using LanguageExt;

namespace Application.Products.Commands;

public record DeleteProductCommand(Guid Id);

public static class DeleteProductCommandHandler
{
    public static async Task<Either<ProductException, Product>> Handle(
        DeleteProductCommand command,
        IProductRepository productRepository,
        IProductQueries productQueries,
        CancellationToken cancellationToken)
    {
        var productId = new ProductId(command.Id);
        var existingOption = await productQueries.GetByIdWithTracking(productId, cancellationToken);

        try
        {
            return await existingOption.Match<Task<Either<ProductException, Product>>>(
                async product =>
                {
                    // EF Core з DeleteBehavior.Cascade автоматично видалить усі прив'язані ProductVariant та ProductPhoto
                    var result = await productRepository.Delete(product, cancellationToken);
                    return result;
                },
                () => Task.FromResult<Either<ProductException, Product>>(new ProductNotFoundException(command.Id)));
        }
        catch (Exception ex)
        {
            return new ProductUnknownException(command.Id, ex);
        }
    }
}