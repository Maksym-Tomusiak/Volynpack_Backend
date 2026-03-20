using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.ProductPhotos;
using Domain.ProductVariants;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductPhotoRepository(ApplicationDbContext context) : IProductPhotoRepository, IProductPhotoQueries
{
    public async Task<ProductPhoto> Add(ProductPhoto photo, CancellationToken cancellationToken)
    {
        await context.ProductPhotos.AddAsync(photo, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return photo;
    }

    public async Task<ProductPhoto> Update(ProductPhoto photo, CancellationToken cancellationToken)
    {
        context.ProductPhotos.Update(photo);
        await context.SaveChangesAsync(cancellationToken);
        return photo;
    }

    public async Task<ProductPhoto> Delete(ProductPhoto photo, CancellationToken cancellationToken)
    {
        context.ProductPhotos.Remove(photo);
        await context.SaveChangesAsync(cancellationToken);
        return photo;
    }

    public async Task<Option<ProductPhoto>> GetById(ProductPhotoId id, CancellationToken cancellationToken)
    {
        var entity = await context.ProductPhotos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<ProductPhoto>.None : Option<ProductPhoto>.Some(entity);
    }

    public async Task<IReadOnlyList<ProductPhoto>> GetByVariantId(ProductVariantId variantId, CancellationToken cancellationToken)
    {
        return await context.ProductPhotos
            .AsNoTracking()
            .Where(x => x.ProductVariantId == variantId)
            .OrderByDescending(x => x.IsPrimary) // Головне фото буде першим у списку
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<ProductPhoto>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken)
    {
        var query = context.ProductPhotos
            .AsNoTracking()
            .AsQueryable();

        // Пошук для фото зазвичай не має великого сенсу, але залишаємо структуру пагінації
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm;
            query = query.Where(x => EF.Functions.ILike(x.PhotoUrl, $"%{term}%"));
        }

        query = query.OrderBy(x => x.Id);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ProductPhoto>(items, totalCount, parameters.PageNumber, parameters.PageSize, totalPages);
    }
}