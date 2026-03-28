using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Domain.ConsultationRequest;

namespace Application.ConsultationRequests.Exceptions;

public record GetConsultationRequestsPaginatedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false);

public static class GetConsultationRequestsPaginatedQueryHandler
{
    public static async Task<PaginatedResult<ConsultationRequest>> Handle(
        GetConsultationRequestsPaginatedQuery query,
        IConsultationRequestQueries queries,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters(
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending);

        return await queries.GetPaginated(parameters, cancellationToken);
    }
}