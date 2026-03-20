using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ConsultationRequests.Exceptions;
using Domain.ConsultationRequest;
using LanguageExt;

namespace Application.ConsultationRequests.Commands;

public record UpdateConsultationRequestCommand(Guid Id, string PhoneNumber, bool IsActive);

public static class UpdateConsultationRequestCommandHandler
{
    public static async Task<Either<ConsultationRequestException, ConsultationRequest>> Handle(
        UpdateConsultationRequestCommand command,
        IConsultationRequestQueries queries,
        IConsultationRequestRepository repository,
        CancellationToken cancellationToken)
    {
        var id = new ConsultationRequestId(command.Id);
        var existingOption = await queries.GetById(id, cancellationToken);

        if (existingOption.IsNone)
            return new ConsultationRequestNotFoundException(command.Id);

        var existing = existingOption.IfNoneUnsafe(() => null!);

        try
        {
            existing.Update(command.PhoneNumber, command.IsActive);
            var result = await repository.Update(existing, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ConsultationRequestUnknownException(command.Id, ex);
        }
    }
}