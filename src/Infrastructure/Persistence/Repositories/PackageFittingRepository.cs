using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.PackageFittings;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PackageFittingRepository(ApplicationDbContext context) : IPackageFittingRepository, IPackageFittingQueries
{
    public async Task<PackageFitting> Add(PackageFitting fitting, CancellationToken cancellationToken)
    {
        await context.PackageFittings.AddAsync(fitting, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return fitting;
    }

    public async Task<PackageFitting> Update(PackageFitting fitting, CancellationToken cancellationToken)
    {
        context.PackageFittings.Update(fitting);
        await context.SaveChangesAsync(cancellationToken);
        return fitting;
    }

    public async Task<PackageFitting> Delete(PackageFitting fitting, CancellationToken cancellationToken)
    {
        context.PackageFittings.Remove(fitting);
        await context.SaveChangesAsync(cancellationToken);
        return fitting;
    }

    public async Task<IReadOnlyList<PackageFitting>> GetAll(CancellationToken cancellationToken)
    {
        return await context.PackageFittings
            .AsNoTracking()
            .Include(x => x.Type)
            .Include(x => x.Material)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<PackageFitting>> GetById(PackageFittingId id, CancellationToken cancellationToken)
    {
        var entity = await context.PackageFittings
            .AsNoTracking()
            .Include(x => x.Type)
            .Include(x => x.Material)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<PackageFitting>.None : Option<PackageFitting>.Some(entity);
    }

    public async Task<PaginatedResult<PackageFitting>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken)
    {
        var query = context.PackageFittings
            .AsNoTracking()
            .Include(x => x.Type)
            .Include(x => x.Material)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm.ToLower();
            query = query.Where(x =>
                EF.Functions.ILike(x.Type!.Title.Uk, $"%{term}%") ||
                EF.Functions.ILike(x.Type!.Title.En, $"%{term}%") ||
                EF.Functions.ILike(x.Material!.Title.Uk, $"%{term}%") ||
                EF.Functions.ILike(x.Material!.Title.En, $"%{term}%"));
        }

        query = query.OrderBy(x => x.Type!.Title.Uk);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<PackageFitting>(items, totalCount, parameters.PageNumber, parameters.PageSize, totalPages);
    }
}
