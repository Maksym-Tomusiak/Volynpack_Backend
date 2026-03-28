using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Application.News.Exceptions;
using LanguageExt;

namespace Application.News.Queries;

public record GetNewsPaginatedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false);

public static class GetNewsPaginatedQueryHandler
{
    public static async Task<PaginatedResult<Domain.News.News>> Handle(
        GetNewsPaginatedQuery query,
        INewsQueries newsQueries,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters(
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending);

        return await newsQueries.GetPaginated(parameters, cancellationToken);
    }
}

