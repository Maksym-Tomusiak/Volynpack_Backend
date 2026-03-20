using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.ProductCategories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductCategoryRepository(ApplicationDbContext context) : IProductCategoryRepository, IProductCategoryQueries
{
    public async Task<ProductCategory> Add(ProductCategory category, CancellationToken cancellationToken)
    {
        await context.ProductCategories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<ProductCategory> Update(ProductCategory category, CancellationToken cancellationToken)
    {
        context.ProductCategories.Update(category);
        await context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<ProductCategory> Delete(ProductCategory category, CancellationToken cancellationToken)
    {
        context.ProductCategories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<IReadOnlyList<ProductCategory>> GetAll(CancellationToken cancellationToken)
    {
        return await context.ProductCategories
            .AsNoTracking()
            .OrderBy(x => x.Name.Uk)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<ProductCategory>> GetById(ProductCategoryId id, CancellationToken cancellationToken)
    {
        var entity = await context.ProductCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<ProductCategory>.None : Option<ProductCategory>.Some(entity);
    }
    
    public async Task<IReadOnlyList<ProductCategory>> GetByIds(IEnumerable<ProductCategoryId> ids, CancellationToken cancellationToken)
    {
        return await context.ProductCategories
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<ProductCategory>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken)
    {
        var query = context.ProductCategories
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm;
            query = query.Where(x =>
                EF.Functions.ILike(x.Name.Uk, $"%{term}%") ||
                EF.Functions.ILike(x.Name.En, $"%{term}%"));
        }

        query = query.OrderBy(x => x.Name.Uk);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ProductCategory>(items, totalCount, parameters.PageNumber, parameters.PageSize, totalPages);
    }
}