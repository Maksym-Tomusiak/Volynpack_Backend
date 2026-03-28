using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ConsultationRequests.Exceptions;
using Domain.ConsultationRequest;
using LanguageExt;

namespace Application.ConsultationRequests.Commands;

public record DeleteConsultationRequestCommand(Guid Id);

public static class DeleteConsultationRequestCommandHandler
{
    public static async Task<Either<ConsultationRequestException, ConsultationRequest>> Handle(
        DeleteConsultationRequestCommand command,
        IConsultationRequestQueries queries,
        IConsultationRequestRepository repository,
        CancellationToken cancellationToken)
    {
        var id = new ConsultationRequestId(command.Id);
        var existingOption = await queries.GetById(id, cancellationToken);

        if (existingOption.IsNone)
            return new ConsultationRequestNotFoundException(command.Id);

        try
        {
            var result = await repository.Delete(existingOption.IfNoneUnsafe(() => null!), cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ConsultationRequestUnknownException(command.Id, ex);
        }
    }
}