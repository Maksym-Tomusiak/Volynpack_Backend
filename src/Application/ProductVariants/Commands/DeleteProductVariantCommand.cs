using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ProductVariants.Exceptions;
using Domain.ProductVariants;
using LanguageExt;

namespace Application.ProductVariants.Commands;

public record DeleteProductVariantCommand(Guid Id);

public static class DeleteProductVariantCommandHandler
{
    public static async Task<Either<ProductVariantException, ProductVariant>> Handle(
        DeleteProductVariantCommand command,
        IProductVariantRepository variantRepository,
        IProductVariantQueries variantQueries,
        CancellationToken cancellationToken)
    {
        var variantId = new ProductVariantId(command.Id);
        
        // ВИПРАВЛЕНО: Використовуємо GetByIdWithTracking
        var existingOption = await variantQueries.GetByIdWithTracking(variantId, cancellationToken);

        try
        {
            return await existingOption.Match<Task<Either<ProductVariantException, ProductVariant>>>(
                async variant => await variantRepository.Delete(variant, cancellationToken),
                () => Task.FromResult<Either<ProductVariantException, ProductVariant>>(new ProductVariantNotFoundException(command.Id)));
        }
        catch (Exception ex)
        {
            return new ProductVariantUnknownException(command.Id, ex);
        }
    }
}