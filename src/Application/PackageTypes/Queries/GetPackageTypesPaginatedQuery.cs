using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Domain.PackageTypes;

namespace Application.PackageTypes.Queries;

public record GetPackageTypesPaginatedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false);

public static class GetPackageTypesPaginatedQueryHandler
{
    public static async Task<PaginatedResult<PackageType>> Handle(
        GetPackageTypesPaginatedQuery query,
        IPackageTypeQueries packageTypeQueries,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters(
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending);

        return await packageTypeQueries.GetPaginated(parameters, cancellationToken);
    }
}
