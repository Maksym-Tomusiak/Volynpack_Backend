using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Domain.PackageMaterials;

namespace Application.PackageMaterials.Queries;

public record GetPackageMaterialsPaginatedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false);

public static class GetPackageMaterialsPaginatedQueryHandler
{
    public static async Task<PaginatedResult<PackageMaterial>> Handle(
        GetPackageMaterialsPaginatedQuery query,
        IPackageMaterialQueries packageMaterialQueries,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters(
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending);

        return await packageMaterialQueries.GetPaginated(parameters, cancellationToken);
    }
}
