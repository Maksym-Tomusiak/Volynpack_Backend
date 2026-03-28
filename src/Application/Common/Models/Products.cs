using Domain.ProductVariants;

namespace Application.Common.Models;

public enum ProductSortOption
{
    PriceAsc,         // Від найдешевших
    PriceDesc,        // Від найдорожчих
    NameAsc,          // За алфавітом (А-Я)
    NameDesc,         // За алфавітом (Я-А)
    Newest            // Спочатку нові (якщо є поле дати створення)
}

// Параметри, які присилає фронтенд
public record ProductFilterParameters(
    int PageNumber = 1,
    int PageSize = 12,
    List<Guid>? CategoryIds = null,  // Список ID категорій
    List<Guid>? TypeIds = null,      // Список ID типів пакування
    List<Guid>? MaterialIds = null,  // Список ID матеріалів
    decimal? Height = null,          // Точна висота
    decimal? Width = null,           // Точна ширина
    decimal? Depth = null,           // Точна глибина
    int? Density = null,          // Щільність (передаємо як рядок)
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    ProductSortOption SortBy = ProductSortOption.PriceAsc
);

// Модель доступних фільтрів (Фасети), яку ми віддаємо на фронт
public class CatalogFacets
{
    public decimal MinAvailablePrice { get; set; }
    public decimal MaxAvailablePrice { get; set; }
    public List<decimal> AvailableHeights { get; set; } = new();
    public List<decimal> AvailableWidths { get; set; } = new();
    public List<decimal?> AvailableDepths { get; set; } = new();
    public List<int> AvailableDensities { get; set; } = new();
    public List<Guid> AvailableMaterialIds { get; set; } = new();
    public List<Guid> AvailableTypeIds { get; set; } = new();
    // У моделі CatalogFacets додай:
    public List<Guid> AvailableCategoryIds { get; set; } = new List<Guid>();
}

// Комплексна відповідь каталогу
public class CatalogFilterResult
{
    public PaginatedResult<ProductVariant> Products { get; set; }
    public CatalogFacets Facets { get; set; }
}