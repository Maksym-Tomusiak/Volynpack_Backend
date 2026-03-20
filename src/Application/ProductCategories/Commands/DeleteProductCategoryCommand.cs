using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ProductCategories.Exceptions;
using Domain.ProductCategories;
using LanguageExt;

namespace Application.ProductCategories.Commands;

public record DeleteProductCategoryCommand(Guid Id);

public static class DeleteProductCategoryCommandHandler
{
    public static async Task<Either<ProductCategoryException, ProductCategory>> Handle(
        DeleteProductCategoryCommand command,
        IProductCategoryRepository repository,
        IProductCategoryQueries queries,
        CancellationToken cancellationToken)
    {
        var categoryId = new ProductCategoryId(command.Id);
        var existingOption = await queries.GetById(categoryId, cancellationToken);

        try
        {
            return await existingOption.Match<Task<Either<ProductCategoryException, ProductCategory>>>(
                async category =>
                {
                    // Якщо в тебе є логіка (не видаляти категорію, якщо в ній є товари),
                    // її варто додати сюди перед видаленням, або покластися на Restrict у базі даних.
                    var result = await repository.Delete(category, cancellationToken);
                    return result;
                },
                () => Task.FromResult<Either<ProductCategoryException, ProductCategory>>(new ProductCategoryNotFoundException(command.Id)));
        }
        catch (Exception ex)
        {
            return new ProductCategoryUnknownException(command.Id, ex);
        }
    }
}