using Application.Common.Interfaces.Repositories;
using Application.ProductCategories.Exceptions;
using Domain;
using Domain.ProductCategories;
using LanguageExt;

namespace Application.ProductCategories.Commands;

public record CreateProductCategoryCommand(
    string NameUk,
    string NameEn);

public static class CreateProductCategoryCommandHandler
{
    public static async Task<Either<ProductCategoryException, ProductCategory>> Handle(
        CreateProductCategoryCommand command,
        IProductCategoryRepository repository,
        CancellationToken cancellationToken)
    {
        try
        {
            var name = new LocalizedString(command.NameUk, command.NameEn);
            var category = ProductCategory.New(name);

            var result = await repository.Add(category, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ProductCategoryUnknownException(Guid.Empty, ex);
        }
    }
}