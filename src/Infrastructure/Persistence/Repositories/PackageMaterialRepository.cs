using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.PackageMaterials;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PackageMaterialRepository(ApplicationDbContext context) : IPackageMaterialRepository, IPackageMaterialQueries
{
    public async Task<PackageMaterial> Add(PackageMaterial material, CancellationToken cancellationToken)
    {
        await context.PackageMaterials.AddAsync(material, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return material;
    }

    public async Task<PackageMaterial> Update(PackageMaterial material, CancellationToken cancellationToken)
    {
        context.PackageMaterials.Update(material);
        await context.SaveChangesAsync(cancellationToken);
        return material;
    }

    public async Task<PackageMaterial> Delete(PackageMaterial material, CancellationToken cancellationToken)
    {
        context.PackageMaterials.Remove(material);
        await context.SaveChangesAsync(cancellationToken);
        return material;
    }

    public async Task<IReadOnlyList<PackageMaterial>> GetAll(CancellationToken cancellationToken)
    {
        return await context.PackageMaterials
            .AsNoTracking()
            .OrderBy(x => x.Title.Uk)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<PackageMaterial>> GetById(PackageMaterialId id, CancellationToken cancellationToken)
    {
        var entity = await context.PackageMaterials
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<PackageMaterial>.None : Option<PackageMaterial>.Some(entity);
    }

    public async Task<PaginatedResult<PackageMaterial>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken)
    {
        var query = context.PackageMaterials
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm.ToLower();
            query = query.Where(x =>
                EF.Functions.ILike(x.Title.Uk, $"%{term}%") ||
                EF.Functions.ILike(x.Title.En, $"%{term}%"));
        }

        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            query = parameters.SortBy.ToLower() switch
            {
                "title" => parameters.SortDescending
                    ? query.OrderByDescending(x => x.Title.Uk)
                    : query.OrderBy(x => x.Title.Uk),
                _ => query.OrderBy(x => x.Title.Uk)
            };
        }
        else
        {
            query = query.OrderBy(x => x.Title.Uk);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<PackageMaterial>(items, totalCount, parameters.PageNumber, parameters.PageSize, totalPages);
    }
}
