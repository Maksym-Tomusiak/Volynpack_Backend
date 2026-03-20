using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.Products;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext context) : IProductRepository, IProductQueries
{
    public async Task<Product> Add(Product product, CancellationToken cancellationToken)
    {
        // ВАЖЛИВО: Оскільки категорії ми дістали через інший запит (можливо AsNoTracking), 
        // нам потрібно повідомити контексту, що ці категорії вже існують в базі, 
        // щоб він не намагався їх перестворити, а лише створив зв'язок (ProductCategoryProduct).
        foreach (var category in product.Categories)
        {
            context.Attach(category); 
        }

        await context.Products.AddAsync(product, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product> Update(Product product, CancellationToken cancellationToken)
    {
        // Так само підтягуємо стейт категорій
        foreach (var category in product.Categories)
        {
            if (context.Entry(category).State == EntityState.Detached)
            {
                context.Attach(category);
            }
        }

        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product> Delete(Product product, CancellationToken cancellationToken)
    {
        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }

    // ДОДАНО INCLUDE
    public async Task<Option<Product>> GetById(ProductId id, CancellationToken cancellationToken)
    {
        var entity = await context.Products
            .AsNoTracking()
            .Include(p => p.Categories) // Обов'язково для DTO
            .Include(p => p.Type)       // Обов'язково для DTO
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<Product>.None : Option<Product>.Some(entity);
    }
    
    // Цей метод використовується при Update
    public async Task<Option<Product>> GetByIdWithTracking(ProductId id, CancellationToken cancellationToken)
    {
        var entity = await context.Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return entity is null ? Option<Product>.None : Option<Product>.Some(entity);
    }

    // ДОДАНО INCLUDE ДЛЯ КАТЕГОРІЙ І ТИПУ В ПАГІНАЦІЇ
    public async Task<PaginatedResult<Product>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken)
    {
        var query = context.Products
            .AsNoTracking()
            .Include(p => p.Categories)
            .Include(p => p.Type)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm;
            query = query.Where(x =>
                EF.Functions.ILike(x.Title.Uk, $"%{term}%") ||
                EF.Functions.ILike(x.Title.En, $"%{term}%"));
        }

        query = query.OrderBy(x => x.Title.Uk);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Product>(items, totalCount, parameters.PageNumber, parameters.PageSize, totalPages);
    }
}