using Application.Common.Interfaces.Repositories;
using Application.ConsultationRequests.Exceptions;
using Domain.ConsultationRequest;
using LanguageExt;

namespace Application.ConsultationRequests.Commands;

public record CreateConsultationRequestCommand(string PhoneNumber);

public static class CreateConsultationRequestCommandHandler
{
    public static async Task<Either<ConsultationRequestException, ConsultationRequest>> Handle(
        CreateConsultationRequestCommand command,
        IConsultationRequestRepository repository,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = ConsultationRequest.New(command.PhoneNumber);
            var result = await repository.Add(request, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ConsultationRequestUnknownException(Guid.Empty, ex);
        }
    }
}