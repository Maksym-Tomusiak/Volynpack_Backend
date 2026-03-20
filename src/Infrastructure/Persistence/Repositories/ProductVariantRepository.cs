using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.PackageMaterials;
using Domain.PackageTypes;
using Domain.ProductCategories;
using Domain.Products;
using Domain.ProductVariants;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductVariantRepository(ApplicationDbContext context) : IProductVariantRepository, IProductVariantQueries
{
    public async Task<ProductVariant> Add(ProductVariant variant, CancellationToken cancellationToken)
    {
        // Optimized: Changed AddAsync to Add
        context.ProductVariants.Add(variant);
        await context.SaveChangesAsync(cancellationToken);
        return variant;
    }

    public async Task<ProductVariant> Update(ProductVariant variant, CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
        return variant;
    }

    public async Task<ProductVariant> Delete(ProductVariant variant, CancellationToken cancellationToken)
    {
        context.ProductVariants.Remove(variant);
        await context.SaveChangesAsync(cancellationToken);
        return variant;
    }

    public async Task<Option<ProductVariant>> GetById(ProductVariantId id, CancellationToken cancellationToken)
    {
        var entity = await context.ProductVariants
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Material)
            .Include(x => x.Photos)
            .AsSplitQuery() // Added for safety if Product has its own collections
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<ProductVariant>.None : Option<ProductVariant>.Some(entity);
    }

    public async Task<IReadOnlyList<ProductVariant>> GetByProductId(ProductId productId,
        CancellationToken cancellationToken)
    {
        return await context.ProductVariants
            .AsNoTracking()
            .Include(x => x.Product)
                .ThenInclude(p => p.Categories)
            .Include(x => x.Material)
            .Include(x => x.Photos)
            .AsSplitQuery()
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.PricePerPiece)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<ProductVariant>> GetPaginated(PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        var baseQuery = context.ProductVariants.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm;
            baseQuery = baseQuery.Where(x =>
                EF.Functions.ILike(x.Product!.Title.Uk, $"%{term}%") ||
                EF.Functions.ILike(x.Product!.Title.En, $"%{term}%") ||
                EF.Functions.ILike(x.Material!.Title.Uk, $"%{term}%") ||
                EF.Functions.ILike(x.Material!.Title.En, $"%{term}%"));
        }

        // Optimized: Count BEFORE includes
        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        // Apply Includes and Pagination after counting
        var items = await baseQuery
            .Include(x => x.Product)
            .Include(x => x.Material)
            .Include(x => x.Photos)
            .AsSplitQuery() // Added to prevent Cartesian explosion
            .OrderBy(x => x.Product!.Title.Uk).ThenBy(x => x.PricePerPiece)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ProductVariant>(items, totalCount, parameters.PageNumber, parameters.PageSize,
            totalPages);
    }

    public async Task<CatalogFilterResult> GetFilteredCatalog(ProductFilterParameters parameters,
        CancellationToken cancellationToken)
    {
        var filterQuery = context.ProductVariants.AsNoTracking().AsQueryable();

        // 1. ВИПРАВЛЕННЯ ПОМИЛКИ LINQ: Створюємо списки строго типізованих ID
        var typeIds = parameters.TypeIds?.Select(id => new PackageTypeId(id)).ToList();
        var materialIds = parameters.MaterialIds?.Select(id => new PackageMaterialId(id)).ToList();
        var categoryIds = parameters.CategoryIds?.Select(id => new ProductCategoryId(id)).ToList();

        if (typeIds != null && typeIds.Any())
            filterQuery = filterQuery.Where(x => typeIds.Contains(x.Product!.PackageTypeId));

        if (materialIds != null && materialIds.Any())
            filterQuery = filterQuery.Where(x => materialIds.Contains(x.PackageMaterialId));

        if (parameters.Height.HasValue) filterQuery = filterQuery.Where(x => x.Height == parameters.Height.Value);
        if (parameters.Width.HasValue) filterQuery = filterQuery.Where(x => x.Width == parameters.Width.Value);
        if (parameters.Depth.HasValue) filterQuery = filterQuery.Where(x => x.Depth == parameters.Depth.Value);
        if (parameters.Density.HasValue) filterQuery = filterQuery.Where(x => x.Density == parameters.Density.Value);

        // 2. ВИПРАВЛЕННЯ ЦІНИ: Тепер фільтруємо за ЦІНОЮ ЗА УПАКОВКУ
        if (parameters.MinPrice.HasValue)
            filterQuery = filterQuery.Where(x => (x.PricePerPiece * x.QuantityPerPackage) >= parameters.MinPrice.Value);
        if (parameters.MaxPrice.HasValue)
            filterQuery = filterQuery.Where(x => (x.PricePerPiece * x.QuantityPerPackage) <= parameters.MaxPrice.Value);

        if (categoryIds != null && categoryIds.Any())
        {
            filterQuery = filterQuery.Where(x => x.Product!.Categories.Any(c => categoryIds.Contains(c.Id)));
        }

        // 3. ФАСЕТИ (тепер також рахують мінімум/максимум ЗА УПАКОВКУ)
        var facets = new CatalogFacets
        {
            MinAvailablePrice =
                await filterQuery.MinAsync(x => (decimal?)(x.PricePerPiece * x.QuantityPerPackage),
                    cancellationToken) ?? 0,
            MaxAvailablePrice =
                await filterQuery.MaxAsync(x => (decimal?)(x.PricePerPiece * x.QuantityPerPackage),
                    cancellationToken) ?? 0,
            AvailableHeights = await filterQuery.Select(x => x.Height).Distinct().ToListAsync(cancellationToken),
            AvailableWidths = await filterQuery.Select(x => x.Width).Distinct().ToListAsync(cancellationToken),
            AvailableDepths = await filterQuery.Where(x => x.Depth.HasValue).Select(x => x.Depth).Distinct()
                .ToListAsync(cancellationToken),
            AvailableDensities = await filterQuery.Select(x => x.Density).Distinct().ToListAsync(cancellationToken),
            AvailableMaterialIds = await filterQuery.Select(x => x.PackageMaterialId.Value).Distinct()
                .ToListAsync(cancellationToken),
            AvailableTypeIds = await filterQuery.Where(x => x.Product != null)
                .Select(x => x.Product!.PackageTypeId.Value).Distinct().ToListAsync(cancellationToken),
            // Додаємо категорії для фронтенду
            AvailableCategoryIds = await filterQuery.SelectMany(x => x.Product!.Categories.Select(c => c.Id.Value))
                .Distinct().ToListAsync(cancellationToken)
        };

        var totalCount = await filterQuery.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        var orderedQuery = parameters.SortBy switch
        {
            ProductSortOption.PriceAsc => filterQuery.OrderBy(x => x.PricePerPiece * x.QuantityPerPackage)
                .ThenBy(x => x.Id),
            ProductSortOption.PriceDesc => filterQuery.OrderByDescending(x => x.PricePerPiece * x.QuantityPerPackage)
                .ThenBy(x => x.Id),
            ProductSortOption.NameAsc => filterQuery.OrderBy(x => x.Product!.Title.Uk).ThenBy(x => x.Id),
            ProductSortOption.NameDesc => filterQuery.OrderByDescending(x => x.Product!.Title.Uk).ThenBy(x => x.Id),
            _ => filterQuery.OrderByDescending(x => x.IsPopular).ThenBy(x => x.PricePerPiece * x.QuantityPerPackage)
                .ThenBy(x => x.Id)
        };

        var items = await orderedQuery
            .Include(x => x.Product)
            .ThenInclude(p => p.Categories)
            .Include(x => x.Material)
            .Include(x => x.Photos)
            .AsSplitQuery()
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new CatalogFilterResult
        {
            Products = new PaginatedResult<ProductVariant>(items, totalCount, parameters.PageNumber,
                parameters.PageSize, totalPages),
            Facets = facets
        };
    }

    public async Task<PaginatedResult<ProductVariant>> GetPopularPaginated(PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        var baseQuery = context.ProductVariants
            .AsNoTracking()
            .Where(x => x.IsPopular);

        // Optimized: Count before Includes
        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        var items = await baseQuery
            .Include(x => x.Product)
                .ThenInclude(p => p.Categories)
            .Include(x => x.Material)
            .Include(x => x.Photos)
            .AsSplitQuery() // Added
            .OrderBy(x => x.PricePerPiece)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ProductVariant>(items, totalCount, parameters.PageNumber, parameters.PageSize, totalPages);
    }

    public async Task<IReadOnlyList<ProductVariant>> GetRelated(ProductId targetProductId, int limit,
        CancellationToken cancellationToken)
    {
        // 1. Отримуємо цільовий продукт із категоріями
        var targetProduct = await context.Products
            .AsNoTracking()
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == targetProductId, cancellationToken);

        if (targetProduct == null)
            return new List<ProductVariant>();

        var targetCategoryIds = targetProduct.Categories.Select(c => c.Id).ToList();
        List<ProductId> matchingProductIds;

        // 2. Шукаємо СХОЖІ ПРОДУКТИ (базові)
        if (targetCategoryIds.Any())
        {
            var allOtherProducts = await context.Products
                .AsNoTracking()
                .Where(p => p.Id != targetProductId)
                .Select(p => new
                {
                    ProductId = p.Id,
                    CategoryIds = p.Categories.Select(c => c.Id).ToList()
                })
                .ToListAsync(cancellationToken);

            matchingProductIds = allOtherProducts
                .Select(p => new
                {
                    p.ProductId,
                    MatchCount = p.CategoryIds.Count(cId => targetCategoryIds.Contains(cId))
                })
                .Where(p => p.MatchCount > 0)
                .OrderByDescending(p => p.MatchCount)
                // Беремо базу з запасом, щоб точно вистачило варіацій
                .Take(limit)
                .Select(p => p.ProductId)
                .ToList();
        }
        else
        {
            matchingProductIds = new List<ProductId>();
        }

        // ЗАПАСНИЙ ВАРІАНТ: якщо спільних категорій немає
        if (!matchingProductIds.Any())
        {
            matchingProductIds = await context.Products
                .AsNoTracking()
                .Where(p => p.Id != targetProductId)
                .Take(limit)
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        if (!matchingProductIds.Any())
            return new List<ProductVariant>();

        // 3. Завантажуємо ВСІ варіації для знайдених базових продуктів
        var variants = await context.ProductVariants
            .AsNoTracking()
            .Include(v => v.Product)
            .ThenInclude(p => p.Categories)
            .Include(v => v.Material)
            .Include(v => v.Photos)
            .AsSplitQuery()
            .Where(v => matchingProductIds.Contains(v.ProductId))
            .ToListAsync(cancellationToken);

        // 4. Обробка в пам'яті (ВИПРАВЛЕНО)
        // Тепер ми не групуємо їх по ProductId, а просто виводимо всі варіації
        var result = variants
            // Сортуємо так, щоб спочатку йшли варіації найбільш релевантних продуктів
            .OrderBy(v => matchingProductIds.IndexOf(v.ProductId))
            // А всередині одного продукту - сортуємо від найдешевшої варіації
            .ThenBy(v => v.PricePerPiece)
            // Обмежуємо загальну кількість ВАРІАЦІЙ лімітом (наприклад, 10 карток у слайдері)
            .Take(limit)
            .ToList();

        return result;
    }

    public async Task<Option<ProductVariant>> GetByIdWithTracking(ProductVariantId id, CancellationToken cancellationToken)
    {
        var entity = await context.ProductVariants
            .Include(x => x.Photos) 
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<ProductVariant>.None : Option<ProductVariant>.Some(entity);
    }

    public async Task<Option<ProductVariant>> GetBySeoUrl(string seoUrl, CancellationToken cancellationToken)
    {
        var entity = await context.ProductVariants
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Material)
            .Include(x => x.Photos)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x =>
                x.SeoUrl.Uk == seoUrl || x.SeoUrl.En == seoUrl, cancellationToken);
                
        return entity is null ? Option<ProductVariant>.None : Option<ProductVariant>.Some(entity);
    }
}