using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Domain.ProductPhotos;

namespace Application.ProductPhotos.Queries;

public record GetProductPhotoPaginatedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false);

public static class GetProductPhotoPaginatedQueryHandler
{
    public static async Task<PaginatedResult<ProductPhoto>> Handle(
        GetProductPhotoPaginatedQuery query,
        IProductPhotoQueries photoQueries,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters(
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending);

        return await photoQueries.GetPaginated(parameters, cancellationToken);
    }
}