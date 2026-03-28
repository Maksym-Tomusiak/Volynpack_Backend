using Domain.ProductCategories;

namespace Api.Dtos.ProductCategories;

public record ProductCategoryDto(Guid Id, LocalizedStringDto Name)
{
    public static ProductCategoryDto FromDomainModel(ProductCategory category) =>
        new(category.Id.Value, new LocalizedStringDto(category.Name.Uk, category.Name.En));
}

public record ProductCategoryCreateDto(string NameUk, string NameEn);

public record ProductCategoryUpdateDto(string NameUk, string NameEn);