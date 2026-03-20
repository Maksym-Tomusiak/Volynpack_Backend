using Application.Common.Interfaces.Queries;
using Application.ConsultationRequests.Exceptions;
using Domain.ConsultationRequest;
using LanguageExt;

namespace Application.ConsultationRequests.Queries;

public record GetConsultationRequestByIdQuery(Guid Id);

public static class GetConsultationRequestByIdQueryHandler
{
    public static async Task<Either<ConsultationRequestException, ConsultationRequest>> Handle(
        GetConsultationRequestByIdQuery query,
        IConsultationRequestQueries queries,
        CancellationToken cancellationToken)
    {
        var id = new ConsultationRequestId(query.Id);
        var result = await queries.GetById(id, cancellationToken);

        return result.Match<Either<ConsultationRequestException, ConsultationRequest>>(
            request => request,
            () => new ConsultationRequestNotFoundException(query.Id));
    }
}