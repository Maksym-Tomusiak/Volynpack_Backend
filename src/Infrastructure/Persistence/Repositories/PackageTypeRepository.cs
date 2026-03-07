using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.PackageTypes;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PackageTypeRepository(ApplicationDbContext context) : IPackageTypeRepository, IPackageTypeQueries
{
    public async Task<PackageType> Add(PackageType packageType, CancellationToken cancellationToken)
    {
        await context.PackageTypes.AddAsync(packageType, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return packageType;
    }

    public async Task<PackageType> Update(PackageType packageType, CancellationToken cancellationToken)
    {
        context.PackageTypes.Update(packageType);
        await context.SaveChangesAsync(cancellationToken);
        return packageType;
    }

    public async Task<PackageType> Delete(PackageType packageType, CancellationToken cancellationToken)
    {
        context.PackageTypes.Remove(packageType);
        await context.SaveChangesAsync(cancellationToken);
        return packageType;
    }

    public async Task<IReadOnlyList<PackageType>> GetAll(CancellationToken cancellationToken)
    {
        return await context.PackageTypes
            .AsNoTracking()
            .OrderBy(x => x.Title.Uk)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<PackageType>> GetById(PackageTypeId id, CancellationToken cancellationToken)
    {
        var entity = await context.PackageTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<PackageType>.None : Option<PackageType>.Some(entity);
    }

    public async Task<PaginatedResult<PackageType>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken)
    {
        var query = context.PackageTypes
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

        return new PaginatedResult<PackageType>(items, totalCount, parameters.PageNumber, parameters.PageSize, totalPages);
    }
}
