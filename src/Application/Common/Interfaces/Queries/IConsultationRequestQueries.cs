using Application.Common.Models;
using Domain.ConsultationRequest;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IConsultationRequestQueries
{
    Task<Option<ConsultationRequest>> GetById(ConsultationRequestId id, CancellationToken cancellationToken);
    Task<PaginatedResult<ConsultationRequest>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
}