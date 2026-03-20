using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ProductCategories.Exceptions;
using Domain;
using Domain.ProductCategories;
using LanguageExt;

namespace Application.ProductCategories.Commands;

public record UpdateProductCategoryCommand(
    Guid Id,
    string NameUk,
    string NameEn);

public static class UpdateProductCategoryCommandHandler
{
    public static async Task<Either<ProductCategoryException, ProductCategory>> Handle(
        UpdateProductCategoryCommand command,
        IProductCategoryRepository repository,
        IProductCategoryQueries queries,
        CancellationToken cancellationToken)
    {
        var categoryId = new ProductCategoryId(command.Id);
        var existingOption = await queries.GetById(categoryId, cancellationToken);

        if (existingOption.IsNone)
            return new ProductCategoryNotFoundException(command.Id);

        try
        {
            var category = existingOption.First();
            var newName = new LocalizedString(command.NameUk, command.NameEn);
            
            category.Update(newName);

            var result = await repository.Update(category, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ProductCategoryUnknownException(command.Id, ex);
        }
    }
}