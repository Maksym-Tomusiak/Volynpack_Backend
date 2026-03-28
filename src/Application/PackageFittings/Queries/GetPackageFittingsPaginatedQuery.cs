using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Domain.PackageFittings;

namespace Application.PackageFittings.Queries;

public record GetPackageFittingsPaginatedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false);

public static class GetPackageFittingsPaginatedQueryHandler
{
    public static async Task<PaginatedResult<PackageFitting>> Handle(
        GetPackageFittingsPaginatedQuery query,
        IPackageFittingQueries packageFittingQueries,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters(
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending);

        return await packageFittingQueries.GetPaginated(parameters, cancellationToken);
    }
}
